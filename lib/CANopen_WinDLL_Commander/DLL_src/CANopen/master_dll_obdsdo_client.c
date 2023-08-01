#include <master_dll_header.h>

#if CHECK_VERSION_CANLIB(3, 0, 1)
//  CHECK_VERSION_CANLIB(2, 3, 1)

static struct objsdo {
	unsigned32 cs;
	unsigned32 sc;
} sdo;

static int16 find_sdo_client_entry(void)
{
	if ((sdo.cs & CAN_MASK_COBID_INVALID) == 0 && (sdo.sc & CAN_MASK_COBID_INVALID) == 0) return CAN_RETOK;
	return CAN_ERRET_SDO_INVALID;
}

int16 find_sdo_client_recv_canid(canlink *canid)
{
	int16 fnr;

	fnr = find_sdo_client_entry();
	if (fnr < 0) return fnr;
	*canid = sdo.sc & CAN_MASK_CANID;
	return CAN_RETOK;
}

int16 find_sdo_client_send_canid(canlink *canid)
{
	int16 fnr;

	fnr = find_sdo_client_entry();
	if (fnr < 0) return fnr;
	*canid = sdo.cs & CAN_MASK_CANID;
	return CAN_RETOK;
}

int16 read_sdo_client_data(cansubind subind, unsigned32 *data)
{
	*data = 0;
	if (subind == 0) *data = 2;
	else if (subind == 1) *data = sdo.cs;
	else if (subind == 2) *data = sdo.sc;
	else return CAN_ERRET_OBD_NOSUBIND;
	return CAN_RETOK;
}

int16 write_sdo_client_data(cansubind subind, unsigned32 data)
{
	if (subind == 0) return CAN_ERRET_OBD_READONLY;
	if (subind > 2) return CAN_ERRET_OBD_NOSUBIND;
	if (find_sdo_client_entry() == CAN_RETOK && (data & CAN_MASK_COBID_INVALID) == 0) return CAN_ERRET_OBD_OBJACCESS;
	if ((data & CAN_MASK_IDSIZE) != 0) return CAN_ERRET_OBD_VALRANGE;
	if (check_sdo_canid(subind, (canlink)data) == RESTRICTED) return CAN_ERRET_OBD_VALRANGE;	// 2.2.1
	data &= (CAN_MASK_COBID_INVALID | MASK_SDO_COBID_DYNAMIC | CAN_MASK_CANID);		// 2.3.1
	if (subind == 1) sdo.cs = data;
	else if (subind == 2) sdo.sc = data;
	return CAN_RETOK;
}

void can_init_sdo_client (void)
{
	sdo.cs = CAN_MASK_COBID_INVALID;
	sdo.sc = CAN_MASK_COBID_INVALID;
}

#endif
