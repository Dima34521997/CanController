#include <master_header.h>

static void monitor(void)
{

//	ci_propogate_sigs();
//  Propogate CAN driver signals for pure polling or signal/polling mode.
//  For RX signal mode CAN receive frame RE-ENTRANT interrupts
//  must be enabled within the CiRead(...) driver function.

	canopen_monitor();
	application_monitor_routine();

	can_sleep(10*CAN_SLEEP_ONE_MILLISEC);
// Sleep is used ONLY to lower CPU load in OS related applications
// NOT needed for microcontroller applications
}

int main(void)
{
	if (start_can_master() != CAN_RETOK) {
		close_logger();
		return CAN_ERRET_MAIN;
	}
	while (TRUE) monitor();
	if (stop_can_master() != CAN_RETOK) {
		close_logger();
		return CAN_ERRET_MAIN;
	}
	return CAN_RETURN_MAIN;
}
