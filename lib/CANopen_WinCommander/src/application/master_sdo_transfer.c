#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 2)

static void set_sdo_dictionary(cannode node)  // 1.1.2 some changes
{
	write_sdo_client_data(1, ((CAN_CANID_SDO_CS_BASE+node) | CAN_MASK_COBID_INVALID) );
	write_sdo_client_data(2, ((CAN_CANID_SDO_SC_BASE+node) | MASK_SDO_COBID_DYNAMIC) );
	write_sdo_client_data(1, ((CAN_CANID_SDO_CS_BASE+node) | MASK_SDO_COBID_DYNAMIC) );
}

// 1.1.1 renamed
int16 read_device_object_sdo(cannode node, canindex index, cansubind subind, canbyte *data, unsigned32 datasize)
{
	struct sdocltappl ca;

	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return CAN_TRANSTATE_SDO_NODE;
	set_sdo_dictionary(node);
	ca.operation = CAN_SDOPER_UPLOAD;
	ca.si.index =  index;
	ca.si.subind = subind;
	ca.datapnt = data;
	ca.datasize = datasize;
	can_sdo_client_transfer(&ca);
	if (ca.ss.state != CAN_TRANSTATE_OK) {
		node_event(node, EVENT_CLASS_NODE_SDO, EVENT_TYPE_ERROR, ca.ss.state, ca.ss.abortcode);
	}
	return ca.ss.state;
}

// 1.1.1 renamed
int16 write_device_object_sdo(cannode node, canindex index, cansubind subind, canbyte *data, unsigned32 datasize)
{
	struct sdocltappl ca;

	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return CAN_TRANSTATE_SDO_NODE;
	set_sdo_dictionary(node);
	ca.operation = CAN_SDOPER_DOWNLOAD;
	ca.si.index =  index;
	ca.si.subind = subind;
	ca.datapnt = data;
	ca.datasize = datasize;
	can_sdo_client_transfer(&ca);
	if (ca.ss.state != CAN_TRANSTATE_OK) {
		node_event(node, EVENT_CLASS_NODE_SDO, EVENT_TYPE_ERROR, ca.ss.state, ca.ss.abortcode);
	}
	return ca.ss.state;
}

#endif
