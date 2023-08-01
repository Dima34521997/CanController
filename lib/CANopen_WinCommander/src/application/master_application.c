#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 1)

static void reset_can_network(void)
{
	cannode node;

	can_sleep(500000);	// Node(s) may produce bootup after first startup - discard
	for (node = CAN_NODE_ID_MIN; node <= CAN_NODE_ID_MAX; node++) {
		can_node[node].nmt_state = CAN_NODE_STATE_DUMMY;
		can_node[node].ecpcnt = 1 + (1000 * CAN_RESET_NODE_MS / CAN_TIMERUSEC);
		nmt_master_command(CAN_NMT_RESET_NODE, node);
		if (can_node[node].node_status == ON) {
			node_event(node, EVENT_CLASS_NODE_STATE, EVENT_TYPE_INFO, EVENT_CODE_NODE_RESET, EVENT_INFO_DUMMY);
		}
		push_logger();
		can_sleep(10*CAN_SLEEP_ONE_MILLISEC);	// 10 Kbit/S match
	}
}

void reset_can_node(cannode node)
{
	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return;
	if (can_node[node].node_status == OFF) return;
	can_node[node].nmt_state = CAN_NODE_STATE_DUMMY;
	can_node[node].ecpcnt = 1 + (1000 * CAN_RESET_NODE_MS / CAN_TIMERUSEC);
	nmt_master_command(CAN_NMT_RESET_NODE, node);
}

void application_timer_routine(void)
{
}

void application_monitor_routine(void)
{
	configure_can_nodes();
	push_logger();
}

void start_can_network(void)
{
	master_event(EVENT_CLASS_MASTER_STATUS, EVENT_TYPE_INFO, EVENT_CODE_MASTER_RUNNING, EVENT_INFO_DUMMY);
	reset_can_network();

//	write_sync_data(CAN_INDEX_SYNC_PERIOD, 0);
//	write_sync_data(CAN_INDEX_SYNC_OVERFLOW, 17);
//	write_sync_data(CAN_INDEX_SYNC_PERIOD, 1000000);
//	write_sync_data(CAN_INDEX_SYNC_COBID, MASK_SYNC_GENERATE | CAN_COBID_SYNC);
}

void init_defaults(void)	// 1.1.1 some changes
{
	cannode node;

	can_network = CAN_NETWORK_CONTROLLER;
	bitrate_index = CAN_BITRATE_INDEX;
	transform_file_name(log_file_name, MASTER_LOG_FILE_NAME_DEF);
	for (node = 0; node <= CAN_NODE_ID_MAX; node++) {
		memset(can_node+node, 0, sizeof(struct canopennode));
		can_node[node].node_status = OFF;
		can_node[node].nmt_state = CAN_NODE_STATE_DUMMY;
	}
	can_node[0].node_status = ON;	// Master
	can_node[0].nmt_state = CAN_NODE_STATE_OPERATIONAL;
}

#endif
