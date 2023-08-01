#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 5)

static struct eventcache event_cache[EVENT_CACHE_SIZE];
static struct eventlog event_fifo[EVENT_FIFO_SIZE];

static int16 sem_event, sem_cachefl;
static unsigned16 head_st, tail_st;

FILE *logger_file=NULL;
char active_file_name[STR_FILE_NAME_SIZE];

static unsigned16 log_flag, log_hour;


static void close_log_file(void)
{
	if (logger_file != NULL) {
		fclose(logger_file);
		logger_file = NULL;
		if (log_flag == FALSE) remove(active_file_name);
	}
}

static void process_logger_file(void)		// 1.1.1 fallback to the default log file name on error
{
	time_t ts;
	struct tm tp;

	ts = time(NULL);
	tp = *localtime(&ts);
	if (log_hour != tp.tm_hour || logger_file == NULL) {
		close_log_file();
		time_stamp_file_name(active_file_name, log_file_name, ts);
		logger_file = fopen(active_file_name, "w");
		if (logger_file == NULL) {
			master_event(EVENT_CLASS_MASTER_LOGGER, EVENT_TYPE_ERROR, EVENT_CODE_LOGGER_FOPEN, EVENT_INFO_DUMMY);
			transform_file_name(log_file_name, MASTER_LOG_FILE_NAME_DEF);
			time_stamp_file_name(active_file_name, log_file_name, ts);
			logger_file = fopen(active_file_name, "w");
		}
		log_flag = FALSE;
		log_hour = tp.tm_hour;
	}
}

static void flush_events_cache(void)
{
	unsigned16 head, cnt;

	CAN_CRITICAL_BEGIN
	sem_cachefl++;
	if (sem_cachefl != 0) {
		sem_cachefl--;
		CAN_CRITICAL_END
		return;
	}
	CAN_CRITICAL_END
	for (cnt = 0; cnt < EVENT_CACHE_SIZE; cnt++) {
		if (event_cache[cnt].capture != 255) continue;
		head = head_st+1;
		if (head >= EVENT_FIFO_SIZE) head = 0;
		CAN_CRITICAL_BEGIN
		sem_event++;
    if (head == tail_st) {    // rare event - all the code into critical section
			if (sem_event != 0) {
				sem_event--;
				sem_cachefl--;
				CAN_CRITICAL_END
				return;
			}
			tail_st += 4 + (EVENT_FIFO_SIZE/100);
			if (tail_st >= EVENT_FIFO_SIZE) tail_st -= EVENT_FIFO_SIZE;
			master_event(EVENT_CLASS_MASTER_LOGGER, EVENT_TYPE_ERROR, EVENT_CODE_FIFO_OVERFLOW, EVENT_INFO_DUMMY);
		}
		sem_event--;
		CAN_CRITICAL_END
		event_fifo[head] = event_cache[cnt].ev;
		head_st = head;
		event_cache[cnt].capture = 0;
		event_cache[cnt].busy = -1;
	}
	CAN_CRITICAL_BEGIN
	sem_cachefl--;
	CAN_CRITICAL_END
}

void log_event(struct eventlog *ev)
{
	unsigned16 cnt;

	CAN_CRITICAL_BEGIN
	sem_cachefl++;
	CAN_CRITICAL_END
	for (cnt = 1; cnt < EVENT_CACHE_SIZE; cnt++) {
		CAN_CRITICAL_BEGIN
		event_cache[cnt].busy++;
		if (event_cache[cnt].busy == 0) {
			CAN_CRITICAL_END
			if (event_cache[cnt].capture == 0) {
				event_cache[cnt].capture = 1;
				event_cache[cnt].ev = *ev;
        event_cache[cnt].ev.ts = time(NULL);   // time(...) must be signal and thread safe function
				event_cache[cnt].capture = 255;
				CAN_CRITICAL_BEGIN
				sem_cachefl--;
				CAN_CRITICAL_END
				flush_events_cache();
				return;
			}
		} else {
			event_cache[cnt].busy--;
			CAN_CRITICAL_END
		}
	}
	event_cache[0].ev.ts = time(NULL);
	event_cache[0].busy = 0;
	event_cache[0].capture = 255;
	CAN_CRITICAL_BEGIN
	sem_cachefl--;
	CAN_CRITICAL_END
	flush_events_cache();
}

void push_logger(void)    // called from the main loop
{
  unsigned16 tail;

  flush_events_cache();    // data may be retained into cache due to the fifo overflow
	CAN_CRITICAL_BEGIN
	sem_event++;
	if (sem_event == 0) {
		CAN_CRITICAL_END
		process_logger_file();
		if (logger_file != NULL) {
			while (tail_st != head_st) {
				tail = tail_st+1;
				if (tail >= EVENT_FIFO_SIZE) tail = 0;
				write_event_to_file(&event_fifo[tail], logger_file);
				log_flag = TRUE;
				tail_st = tail;
			}
			fflush(logger_file);
		}
		CAN_CRITICAL_BEGIN
	}
	sem_event--;
	CAN_CRITICAL_END
}

void master_event(unsigned8 cls, unsigned8 type, int16 code, int32 info)
{
	struct eventlog ev;

	memset(&ev, 0, sizeof(struct eventlog));
	ev.node = EVENT_NODE_MASTER;
	ev.cls = cls;
	ev.type = type;
	ev.code = code;
	ev.info = info;
	log_event(&ev);
}

void node_event(cannode node, unsigned8 cls, unsigned8 type, int16 code, int32 info)
{
	struct eventlog ev;

	memset(&ev, 0, sizeof(struct eventlog));
	ev.node = node;
	ev.cls = cls;
	ev.type = type;
	ev.code = code;
	ev.info = info;
	log_event(&ev);
}

void close_logger(void)
{
	push_logger();
	close_log_file();
}

void init_logger(void)
{
	unsigned16 cnt;

	sem_event = 0;
	sem_cachefl = 0;
	for (cnt = 0; cnt < STR_FILE_NAME_SIZE; cnt++) active_file_name[cnt] = 0;
	logger_file = NULL;
	log_flag = FALSE;
	log_hour = 0;
	head_st = 0;
	tail_st = 0;
	for (cnt = 0; cnt < EVENT_CACHE_SIZE; cnt++) {
		memset(&event_cache[cnt], 0, sizeof(struct eventcache));
		event_cache[cnt].capture = 0;
		event_cache[cnt].busy = -1;
	}
	event_cache[0].ev.node = EVENT_NODE_MASTER;
	event_cache[0].ev.cls = EVENT_CLASS_MASTER_LOGGER;
	event_cache[0].ev.type = EVENT_TYPE_ERROR;
	event_cache[0].ev.code = EVENT_CODE_CACHE_OVERFLOW;
	sem_event = -1;
	sem_cachefl = -1;
}

#endif
