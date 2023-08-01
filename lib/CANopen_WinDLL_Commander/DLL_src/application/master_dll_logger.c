#include <master_dll_header.h>

#if CHECK_VERSION_APPL(1, 1, 3)
//  CHECK_VERSION_APPL(1, 0, 1)

static struct eventcache event_cache[EVENT_CACHE_SIZE];
static struct eventlog event_fifo[EVENT_FIFO_SIZE];

static int16 sem_event, sem_cachefl;
static unsigned16 head_st, tail_st;

void flush_events_cache(void)
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
		if (head == tail_st) {		// rare event - all critical section
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
				event_cache[cnt].ev.ts = time(NULL);
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

int16 read_logger_event(struct eventlog *ev)
{
	unsigned16 tail;

	flush_events_cache();
	memset(ev, 0, sizeof(struct eventlog));
	CAN_CRITICAL_BEGIN
	sem_event++;
	if (sem_event != 0) {
		sem_event--;
		CAN_CRITICAL_END
		ev->node = EVENT_NODE_MASTER;
		ev->cls = EVENT_CLASS_MASTER_LOGGER;
		ev->type = EVENT_TYPE_WARNING;
		ev->code = EVENT_CODE_LOGGER_BUSY;
		return GEN_ERRET_LOGGER_BUSY;
	}
	if (tail_st == head_st) {
		sem_event--;
		CAN_CRITICAL_END
		ev->node = EVENT_NODE_MASTER;
		ev->cls = EVENT_CLASS_DUMMY;
		ev->type = EVENT_TYPE_DUMMY;
		ev->code = EVENT_CODE_DUMMY;
		return GEN_ERRET_NO_LOGEVENT;
	}
	CAN_CRITICAL_END
	tail = tail_st+1;
	if (tail >= EVENT_FIFO_SIZE) tail = 0;
	*ev = event_fifo[tail];
	tail_st = tail;
	CAN_CRITICAL_BEGIN
	sem_event--;
	CAN_CRITICAL_END
	return GEN_RETOK;
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

void init_logger(void)
{
	unsigned16 cnt;

	sem_event = 0;
	sem_cachefl = 0;
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
