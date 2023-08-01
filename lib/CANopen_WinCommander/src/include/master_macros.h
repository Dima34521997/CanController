// *** CANopen macros definitions ***

#define	CLEAR_FRAME	msg_zero

#define CHECK_VERSION_CANLIB(MAJ, MIN, REL)   ((MAJ == MAJ_VERS_CANLIB) && (MIN == MIN_VERS_CANLIB) && (REL >= RELEASE_CANLIB))
#define CHECK_VERSION_APPL(MAJ, MIN, REL)   ((MAJ == MAJ_VERS_APPL) && (MIN == MIN_VERS_APPL) && (REL >= RELEASE_APPL))

#define CAN_CRITICAL_INIT    init_critical();
#define CAN_CRITICAL_BEGIN   enter_critical();
#define CAN_CRITICAL_END     leave_critical();
