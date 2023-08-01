#include <master_dll_header.h>

#if CHECK_VERSION_CANLIB(3, 0, 1)
//  CHECK_VERSION_CANLIB(2, 3, 1)

static canindex idx[CAN_CANID_CONFIG_MAX+1];

int16 correct_recv_canid(canindex index, canlink canid)
{
	unsigned16 cnt;

	if (index == CAN_INDEX_DUMMY) return CAN_RETOK;
	canid &= CAN_MASK_CANID;
	if (canid > CAN_CANID_CONFIG_MAX) return CAN_ERRET_OUTOFMEM;
	for (cnt = 0; cnt <= CAN_CANID_CONFIG_MAX; cnt++) {
		if (index == idx[cnt]) idx[cnt] = CAN_INDEX_DUMMY;
	}
	if (canid != CAN_CANID_DUMMY) idx[canid] = index;
	return CAN_RETOK;
}

canindex find_comm_recv_index(canlink canid)   // 3.0.1 API changed
{
	if (canid <= CAN_CANID_CONFIG_MAX) return idx[canid];
	return CAN_INDEX_DUMMY;
}

unsigned8 check_sdo_canid(cansubind subind, canlink canid)   // 2.2.1
{
	canid &= CAN_MASK_CANID;
	if (subind == 1) {
		if (canid < CAN_CANID_SDO_CS_MIN || canid > CAN_CANID_SDO_CS_MAX) return RESTRICTED;
	} else if (subind == 2) {
		if (canid < CAN_CANID_SDO_SC_MIN || canid > CAN_CANID_SDO_SC_MAX) return RESTRICTED;
	}
	return UN_RESTRICTED;
}

unsigned8 check_canid_restricted(canlink canid)		// 2.2.1
{
	canid &= CAN_MASK_CANID;
	if (canid == 0x000) return RESTRICTED;
	if (canid >= 0x001 && canid <= 0x07F) return RESTRICTED;
	if (canid >= 0x101 && canid <= 0x180) return RESTRICTED;
	if (canid >= 0x581 && canid <= 0x5FF) return RESTRICTED;
	if (canid >= 0x601 && canid <= 0x67F) return RESTRICTED;
	if (canid >= 0x6E0 && canid <= 0x6FF) return RESTRICTED;
	if (canid >= 0x701 && canid <= 0x77F) return RESTRICTED;
	if (canid >= 0x780 && canid <= 0x7FF) return RESTRICTED;
	return UN_RESTRICTED;
}

void can_init_recv_canids(void)
{
	unsigned16 cnt;

	for (cnt = 0; cnt <= CAN_CANID_CONFIG_MAX; cnt++) idx[cnt] = CAN_INDEX_DUMMY;
}

#endif
