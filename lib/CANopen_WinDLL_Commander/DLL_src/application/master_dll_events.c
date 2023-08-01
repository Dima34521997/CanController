#include <master_dll_header.h>

#if CHECK_VERSION_APPL(1, 1, 1)
//  CHECK_VERSION_APPL(1, 0, 0)

void consume_sync(unsigned8 sc)
{
}

void no_sync_event(void)
{
	master_event(EVENT_CLASS_SYNC, EVENT_TYPE_ERROR, EVENT_CODE_NO_SYNC, EVENT_INFO_DUMMY);
}

void consume_controller_error(canev ev)
{
	if (ev == CIEV_EWL) {
		master_emcy(CAN_EMCY_EWL);
	} else if (ev == CIEV_WTOUT) {
		master_emcy(CAN_EMCY_WTOUT); 
	} else if (ev == CIEV_HOVR) {
		master_emcy(CAN_EMCY_HOVR);
	} else if (ev == CIEV_SOVR) {
		master_emcy(CAN_EMCY_SOVR); 
	} else if (ev == CIEV_BOFF) {
		master_event(EVENT_CLASS_EMCY, EVENT_TYPE_FATAL, CAN_EMCYREG_BOFF, EVENT_INFO_DUMMY);
	}
}

void master_emcy(unsigned16 errorcode)
{
	master_event(EVENT_CLASS_EMCY, EVENT_TYPE_ERROR, errorcode, EVENT_INFO_DUMMY);
}

void master_emcy_index(unsigned16 errorcode, canindex index)
{
	master_event(EVENT_CLASS_EMCY, EVENT_TYPE_ERROR, errorcode, index);
}

void consume_emcy(canframe *cf)
{
	int16 node;
	unsigned16 emcy;

	node = cf->id - CAN_CANID_EMCY_BASE;
	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return;
	emcy = canframe_to_u16(cf->data);
	if ((emcy & 0xFF00) == CAN_EMCY_NO_ERROR) {
		node_event((cannode)node, EVENT_CLASS_EMCY, EVENT_TYPE_INFO, emcy, EVENT_INFO_DUMMY);
	} else {
		node_event((cannode)node, EVENT_CLASS_EMCY, EVENT_TYPE_ERROR, emcy, EVENT_INFO_DUMMY);
	}
}

void no_pdo_event(canindex index)		// 2.2.1
{
	master_emcy_index(CAN_EMCY_RPDO_TIMEOUT, index);
}

void can_cache_overflow(void)
{
	master_emcy(CAN_EMCYREG_CACHE);
}

#endif
