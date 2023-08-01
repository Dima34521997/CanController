#include <master_header.h>

#define CAN_TIMERUSEC_MIN		100	// Minimum background timer period in microseconds.

struct ThreadArg{
	unsigned int msec;  //millisec
	void (* fun)(void);
};

static VOID APIENTRY TimerAPCfunct(LPVOID, DWORD, DWORD);
static DWORD WINAPI ThreadFunct(void *);
struct ThreadArg ThreadArgs;
HANDLE TimerHandle=NULL;

CRITICAL_SECTION crs;

void can_sleep(int32 microseconds)
{
	if (microseconds <= 0) return;
	Sleep(1+microseconds/1000);
}

void can_init_system_timer(void (*handler)(void))
{
	HANDLE *ThreadHandle;
	DWORD ThreadId;

	if (CAN_TIMERUSEC < CAN_TIMERUSEC_MIN) {
		master_event(EVENT_CLASS_MASTER_TIMER, EVENT_TYPE_FATAL, EVENT_CODE_LOW_PERIOD, EVENT_INFO_DUMMY);
 		return;
	}
	memset(&ThreadArgs, 0, sizeof(struct ThreadArg));
	ThreadArgs.msec = (CAN_TIMERUSEC+500)/1000;
	if (ThreadArgs.msec == 0) ThreadArgs.msec = 1;
	ThreadArgs.fun = handler;
	ThreadHandle = (HANDLE) _beginthreadex(NULL, 0, ThreadFunct, (void *)&ThreadArgs, 0, &ThreadId);
}

static DWORD WINAPI ThreadFunct(void *PArgc)
{
	DWORD Period;
	LARGE_INTEGER DueTime;
	struct ThreadArg *p=(struct ThreadArg *)PArgc;

	Period=p->msec;
	DueTime.QuadPart = -(LONGLONG)Period * 10000;
	if (TimerHandle != NULL){
		CancelWaitableTimer (TimerHandle);
		CloseHandle (TimerHandle);
	}
	TimerHandle = CreateWaitableTimer (NULL, FALSE, NULL);
	if (TimerHandle == NULL){
		master_event(EVENT_CLASS_MASTER_TIMER, EVENT_TYPE_FATAL, EVENT_CODE_NO_CREATE, EVENT_INFO_DUMMY);
 		return -1;
	}
	if (!SetWaitableTimer (TimerHandle, &DueTime,Period,TimerAPCfunct,NULL,TRUE )) {
		master_event(EVENT_CLASS_MASTER_TIMER, EVENT_TYPE_FATAL, EVENT_CODE_NO_SET, EVENT_INFO_DUMMY);
 		return -1;
	}
	while (TRUE) {
		SleepEx (INFINITE, TRUE);
		(*p->fun)();
	}
	CancelWaitableTimer (TimerHandle);
	CloseHandle (TimerHandle);
	return 0;
}

static VOID APIENTRY TimerAPCfunct(LPVOID lpCount,
	DWORD dwTimerLowValue, DWORD dwTimerHighValue)
{
}

void can_cancel_system_timer(void)
{
	CancelWaitableTimer (TimerHandle);
	CloseHandle (TimerHandle);
}

void init_critical(void)
{
	InitializeCriticalSection(&crs);
}

void enter_critical(void)
{
	EnterCriticalSection(&crs);
}

void leave_critical(void)
{
	LeaveCriticalSection(&crs);
}

void enable_can_transmitter(void)
{
}

void disable_can_transmitter(void)
{
}
