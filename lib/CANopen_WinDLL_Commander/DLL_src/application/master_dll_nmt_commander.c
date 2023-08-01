#include <master_dll_header.h>

#if CHECK_VERSION_APPL(1, 1, 1)
//  CHECK_VERSION_APPL(1, 0, 0)

void nmt_master_command(unsigned8 cs, cannode node)
{
	canframe cf;

	msg_zero(&cf);
	cf.id = CAN_CANID_NMT;
	cf.data[0] = cs;
	cf.data[1] = node;
	cf.len = CAN_DATALEN_NMT_MC;
	send_can_data(&cf, CAN_PRIORITY_NMT);
}

void consume_nmt(canframe *cf)
{
	cannode node;
    
	node = (cannode)(cf->id - CAN_CANID_NMT_ERROR_BASE);
	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return;
	can_node[node].nmt_state = cf->data[0] & 0x7F;
	if (can_node[node].nmt_state == CAN_NODE_STATE_INITIALISING) {
		node_event(node, EVENT_CLASS_NODE_STATE, EVENT_TYPE_INFO, EVENT_CODE_NODE_BOOTUP, EVENT_INFO_DUMMY);
	}
	if (can_node[node].ecpvalue != 0) {
		can_node[node].ecpcnt = 1 + (unsigned32)can_node[node].ecpvalue * 1000 / CAN_TIMERUSEC;
	}
}

void manage_master_ecp(void)
{
	cannode node;

	for (node = CAN_NODE_ID_MIN; node <= CAN_NODE_ID_MAX; node++) {
		if (can_node[node].ecpvalue == 0) continue;
		if (can_node[node].ecpcnt != 0) {
			can_node[node].ecpcnt--;
			if (can_node[node].ecpcnt == 0) {
				can_node[node].nmt_state = CAN_NODE_STATE_UNCERTAIN;
				node_event(node, EVENT_CLASS_NODE_STATE, EVENT_TYPE_WARNING, EVENT_CODE_NODE_HEARTBEAT, EVENT_INFO_DUMMY);
			}
		}
	}
}

int16 read_master_hbt(cannode node, unsigned16 *hbt)
{
	*hbt = 0;
	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return CAN_ERRET_NODEID;
	*hbt = can_node[node].ecpvalue;
	return CAN_RETOK;
}

int16 write_master_hbt(cannode node, unsigned16 hbt)
{
	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return CAN_ERRET_NODEID;
	can_node[node].ecpcnt = 0xFFFFFFFF;
	can_node[node].ecpvalue = hbt;
	if (hbt == 0) can_node[node].nmt_state = CAN_NODE_STATE_DUMMY;
	else can_node[node].ecpcnt = 1 + (unsigned32)hbt * 1000 / CAN_TIMERUSEC;
	return CAN_RETOK;
}

unsigned8 read_nmt_state(cannode node)
{
	if (node > CAN_NODE_ID_MAX) return CAN_NODE_STATE_DUMMY;	// 0 master
	return can_node[node].nmt_state;
}

void init_node_status(void)
{
	cannode node;

	for (node = 0; node <= CAN_NODE_ID_MAX; node++) {
		can_node[node].ecpvalue = 0;
		can_node[node].ecpcnt = 0xFFFFFFFF;
		can_node[node].nmt_state = CAN_NODE_STATE_DUMMY;
	}
	can_node[0].nmt_state = CAN_NODE_STATE_OPERATIONAL;	// Master
}

#endif
