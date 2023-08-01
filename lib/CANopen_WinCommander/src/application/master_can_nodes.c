#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 0)

static int16 check_node_valid(cannode node)
{
	unsigned32 devdata;

	if (can_node[node].maskdev & MASK_DEV_DEVICETYPE) {
		if (read_device_object_sdo(node, CAN_INDEX_DEVICE_TYPE, 0, (canbyte*)&devdata, 4) != CAN_TRANSTATE_OK) {
			return NOT_VALID;
		}
		if (devdata != can_node[node].DeviceType) {
			node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_DEVICE_TYPE, devdata);
			return NOT_VALID;
		}
	}
	if (can_node[node].maskdev & MASK_DEV_VENDORID) {
		if (read_device_object_sdo(node, CAN_INDEX_IDENTITY, 1, (canbyte*)&devdata, 4) != CAN_TRANSTATE_OK) {
			return NOT_VALID;
		}
		if (devdata != can_node[node].VendorID) {
			node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_VENDOR_ID, devdata);
			return NOT_VALID;
		}
	}
	if (can_node[node].maskdev & MASK_DEV_PRODUCTCODE) {
		if (read_device_object_sdo(node, CAN_INDEX_IDENTITY, 2, (canbyte*)&devdata, 4) != CAN_TRANSTATE_OK) {
			return NOT_VALID;
		}
		if (devdata != can_node[node].ProductCode) {
			node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_PRODUCT_CODE, devdata);
			return NOT_VALID;
		}
	}
	if (can_node[node].maskdev & MASK_DEV_REVISION) {
		if (read_device_object_sdo(node, CAN_INDEX_IDENTITY, 3, (canbyte*)&devdata, 4) != CAN_TRANSTATE_OK) {
			return NOT_VALID;
		}
		if (devdata != can_node[node].Revision) {
			node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_REVISION, devdata);
			return NOT_VALID;
		}
	}
	if (can_node[node].maskdev & MASK_DEV_SERIAL) {
		if (read_device_object_sdo(node, CAN_INDEX_IDENTITY, 4, (canbyte*)&devdata, 4) != CAN_TRANSTATE_OK) {
			return NOT_VALID;
		}
		if (devdata != can_node[node].Serial) {
			node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_SERIAL, devdata);
			return NOT_VALID;
		}
	}
	return VALID;
}

static void can_node_config(cannode node)
{
	unsigned16 hbt;

	can_node[node].nmt_state = CAN_NODE_STATE_UNCERTAIN;
	if (check_node_valid(node) != VALID) return;
	hbt = CAN_HBT_PRODUCER_MS;
	if (write_device_object_sdo(node, CAN_INDEX_PROD_HBT, 0, (canbyte*)&hbt, 2) != CAN_TRANSTATE_OK) {
		node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_PROD_HBT, EVENT_INFO_DUMMY);
		return;
	}
	can_node[node].nmt_state = CAN_NODE_STATE_PRE_OPERATIONAL;
	node_event(node, EVENT_CLASS_NODE_CONFIG, EVENT_TYPE_INFO, EVENT_CODE_NODE_CONFIGURED, EVENT_INFO_DUMMY);
	nmt_master_command(CAN_NMT_START_REMOTE_NODE, node);
}

void configure_can_nodes(void)
{
	cannode node;

	for (node = CAN_NODE_ID_MIN; node <= CAN_NODE_ID_MAX; node++) {
		if (can_node[node].node_status == ON &&
			can_node[node].nmt_state == CAN_NODE_STATE_INITIALISING) {
			can_node_config(node);
		}
	}
}

#endif
