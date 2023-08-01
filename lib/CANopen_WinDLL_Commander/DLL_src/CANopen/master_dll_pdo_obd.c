#include <master_dll_header.h>

#if CHECK_VERSION_CANLIB(3, 0, 0)
//  CHECK_VERSION_CANLIB(2, 3, 2)

#define MASK_RTR_ALLOWED	0x40000000	// RTR allowed on this PDO bit mask

static struct recvpdo {
	unsigned32 id;
	unsigned8 trtype;
	unsigned16 et;
	int32 etcnt;
	cannode node;
} rcvpdo[CAN_NOF_PDO_RECEIVE];

static struct tranpdo {
	unsigned32 id;
	unsigned8 trtype;
	unsigned16 inh;
	unsigned16 et;
	unsigned8 syncstart;
	unsigned8 resync;
	unsigned8 trcnt;
	unsigned32 inhcnt;
	int32 etcnt;
	cannode node;
} trnpdo[CAN_NOF_PDO_TRANSMIT];

static int16 find_pdo_recv_entry(canindex index, int16 *entry)
{
	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index < CAN_INDEX_RCVPDO_COMM_MIN+CAN_NOF_PDO_RECEIVE) {
		*entry = index - CAN_INDEX_RCVPDO_COMM_MIN;
		if ((rcvpdo[*entry].id & CAN_MASK_COBID_INVALID) == 0) return CAN_RETOK;
		return CAN_ERRET_PDO_INVALID;
	}
	*entry = -1;
	return CAN_ERRET_OBD_NOOBJECT;
}

static int16 find_pdo_tran_entry(canindex index, int16 *entry)
{
	if (index >= CAN_INDEX_TRNPDO_COMM_MIN && index < CAN_INDEX_TRNPDO_COMM_MIN+CAN_NOF_PDO_TRANSMIT) {
		*entry = index - CAN_INDEX_TRNPDO_COMM_MIN;
		if ((trnpdo[*entry].id & CAN_MASK_COBID_INVALID) == 0) return CAN_RETOK;
		return CAN_ERRET_PDO_INVALID;
	}
	*entry = -1;
	return CAN_ERRET_OBD_NOOBJECT;
}

static int16 find_pdo_comm_entry(canindex index, int16 *entry)
{
	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
		return find_pdo_recv_entry(index, entry);
	} else {
		return find_pdo_tran_entry(index, entry);
	}
}

int16 check_pdo_comm_object(canindex index)		// 2.2.1
{
	int16 entry;

	return find_pdo_comm_entry(index, &entry);
}

int16 find_pdo_recv_trantype(canindex index, unsigned8 *trtype)
{
	int16 entry, fnr;

	fnr = find_pdo_recv_entry(index, &entry);
	if (fnr != CAN_RETOK) return fnr;
	*trtype = rcvpdo[entry].trtype;
	return CAN_RETOK;
}

int16 find_pdo_tran_trantype(canindex index, unsigned8 *trtype)
{
	int16 entry, fnr;

	fnr = find_pdo_tran_entry(index, &entry);
	if (fnr != CAN_RETOK) return fnr;
	*trtype = trnpdo[entry].trtype;
	return CAN_RETOK;
}

int16 find_pdo_recv_canid(canindex index, canlink *canid)
{
	int16 entry, fnr;

	fnr = find_pdo_recv_entry(index, &entry);
	if (fnr != CAN_RETOK) return fnr;
	if ((rcvpdo[entry].id & MASK_RTR_ALLOWED) != 0) return CAN_ERRET_PDO_NORTR;
	*canid = rcvpdo[entry].id & CAN_MASK_CANID;
	return CAN_RETOK;
}

int16 find_pdo_tran_canid(canindex index, canlink *canid)
{
	int16 entry, fnr;

	fnr = find_pdo_tran_entry(index, &entry);
	if (fnr != CAN_RETOK) return fnr;
	*canid = trnpdo[entry].id & CAN_MASK_CANID;
	return CAN_RETOK;
}

void find_pdo_rtr_tran_index(canlink canid, canindex *index)
{
	unsigned16 cnt;

	for (cnt = 0; cnt < CAN_NOF_PDO_TRANSMIT; cnt++) {
		if (canid == (trnpdo[cnt].id & CAN_MASK_CANID)) {
			if ( ((trnpdo[cnt].id & CAN_MASK_COBID_INVALID) == 0) &&
			     ((trnpdo[cnt].id & MASK_RTR_ALLOWED) == 0) ) {
				*index = cnt+CAN_INDEX_TRNPDO_COMM_MIN;
				return;
			} else break;
		}
	}
	*index = CAN_INDEX_DUMMY;
}

int16 get_pdo_node(canindex index, cannode *node)
{
	int16 entry;

	*node = 0;
	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
		find_pdo_recv_entry(index, &entry);
		if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
		*node = rcvpdo[entry].node;
	} else {
		find_pdo_tran_entry(index, &entry);
		if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
		*node = trnpdo[entry].node;
	}
	return CAN_RETOK;
}

int16 put_pdo_node(canindex index, cannode node)
{
	int16 entry;

	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
		find_pdo_recv_entry(index, &entry);
		if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
		rcvpdo[entry].node = node;
	} else {
		find_pdo_tran_entry(index, &entry);
		if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
		trnpdo[entry].node = node;
	}
	return CAN_RETOK;
}

int16 set_pdo_state(canindex index, unsigned8 state)		// 2.3.2 some changes
{
	int16 entry, fnr;

	fnr = CAN_RETOK;
	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
		find_pdo_recv_entry(index, &entry);
		if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
		rcvpdo[entry].id |= CAN_MASK_COBID_INVALID;
		if (state == VALID) {
			fnr = check_pdo_map_object((canindex)(index+0x200));
			if (fnr != CAN_RETOK) return fnr;
			fnr = correct_recv_canid(index, (canlink)rcvpdo[entry].id);
			if (fnr == CAN_RETOK) rcvpdo[entry].id &= ~CAN_MASK_COBID_INVALID;
		} else {
			fnr = correct_recv_canid(index, CAN_CANID_DUMMY);
		}
	} else {
		find_pdo_tran_entry(index, &entry);
		if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
		trnpdo[entry].id |= CAN_MASK_COBID_INVALID;
		if (state == VALID) {
			fnr = check_pdo_map_object((canindex)(index+0x200));
			if (fnr != CAN_RETOK) return fnr;
			if ((trnpdo[entry].id & MASK_RTR_ALLOWED) == 0 ) {
				fnr = correct_recv_canid(index, (canlink)trnpdo[entry].id);
			}
			if (fnr == CAN_RETOK) trnpdo[entry].id &= ~CAN_MASK_COBID_INVALID;
		} else {
			fnr = correct_recv_canid(index, CAN_CANID_DUMMY);
		}
	}
	return fnr;
}

void set_all_pdos_state(unsigned8 state)
{
	unsigned16 cnt;

	for (cnt = 0; cnt < CAN_NOF_PDO_RECEIVE; cnt++) {
		set_pdo_state((canindex)(CAN_INDEX_RCVPDO_COMM_MIN + cnt), state);
	}
	for (cnt = 0; cnt < CAN_NOF_PDO_TRANSMIT; cnt++) {
		set_pdo_state((canindex)(CAN_INDEX_TRNPDO_COMM_MIN + cnt), state);
	}
}

static void set_pdo_recv_et(int16 entry)
{
	rcvpdo[entry].etcnt = -1;
	if ((rcvpdo[entry].id & CAN_MASK_COBID_INVALID) != 0) return;		// 2.3.2
	if (rcvpdo[entry].et != 0) {
		rcvpdo[entry].etcnt = 1 + (int32)rcvpdo[entry].et * 1000 / CAN_TIMERUSEC;
	}
}

static void set_pdo_tran_et(int16 entry)
{
	int32 etc;

	trnpdo[entry].etcnt = -1;
	if ((trnpdo[entry].id & CAN_MASK_COBID_INVALID) != 0) return;		// 2.3.2
	if (trnpdo[entry].trtype >= 254 && trnpdo[entry].et != 0) {
		etc = (int32)trnpdo[entry].et * 1000 / CAN_TIMERUSEC;
		if (etc == 0) etc = 1;
		trnpdo[entry].etcnt = etc;
	}
}

void set_pdo_recv_event_timer(canindex index)
{
	int16 entry;

	find_pdo_recv_entry(index, &entry);
	if (entry < 0) return;
	set_pdo_recv_et(entry);
}

void set_pdo_tran_event_timer(canindex index)
{
	int16 entry;

	find_pdo_tran_entry(index, &entry);
	if (entry < 0) return;
	set_pdo_tran_et(entry);
}

int16 test_cyclic_tpdo(canindex index, unsigned8 sc)
{
	int16 entry, fnr;

	fnr = find_pdo_tran_entry(index, &entry);
	if (fnr != CAN_RETOK) return fnr;
	if (trnpdo[entry].trtype == 0 || trnpdo[entry].trtype > 240) return CAN_ERRET_PDO_TRTYPE;
	if (trnpdo[entry].resync == ON) {
		if (trnpdo[entry].syncstart != 0) {
			if (sc != 0 && trnpdo[entry].syncstart != sc) return CAN_ERRET_PDO_TRIGGER;
		}
		trnpdo[entry].trcnt = trnpdo[entry].trtype;
		trnpdo[entry].resync = OFF;
	}
	trnpdo[entry].trcnt--;
	if (trnpdo[entry].trcnt > 0) return CAN_ERRET_PDO_TRIGGER;
	trnpdo[entry].trcnt = trnpdo[entry].trtype;
	return CAN_RETOK;
}

int16 test_tpdo_inhibit(canindex index)
{
	int16 entry, fnr;

	fnr = find_pdo_tran_entry(index, &entry);
	if (fnr != CAN_RETOK) return fnr;
	if (trnpdo[entry].trtype >= 254) {
		if (trnpdo[entry].inhcnt != 0) return CAN_ERRET_PDO_INHIBIT;
		trnpdo[entry].inhcnt = (unsigned32)trnpdo[entry].inh * 100 / CAN_TIMERUSEC;
	}
	return CAN_RETOK;
}

void control_pdo(void)		// 2.3.2 some changes
{
	unsigned16 entry;

	for (entry = 0; entry < CAN_NOF_PDO_RECEIVE; entry++) {
		if (rcvpdo[entry].etcnt > 0) {
			rcvpdo[entry].etcnt--;
			if (rcvpdo[entry].etcnt == 0) {
				set_pdo_recv_et(entry);
				no_pdo_event((canindex)(CAN_INDEX_RCVPDO_COMM_MIN+entry));		// 2.2.2
			}
		}
	}
	for (entry = 0; entry < CAN_NOF_PDO_TRANSMIT; entry++) {
		if (trnpdo[entry].inhcnt > 0) trnpdo[entry].inhcnt--;
		if (trnpdo[entry].etcnt > 0) {
			trnpdo[entry].etcnt--;
			if (trnpdo[entry].etcnt == 0) {
				set_pdo_tran_et(entry);
				transmit_can_pdo((canindex)(CAN_INDEX_TRNPDO_COMM_MIN+entry));
			}
		}
	}
}

static int16 pdo_comm_bytes_objsize(cansubind subind)
{
	switch (subind) {
		case 0:	return sizeof(unsigned8);
		case 1:	return sizeof(unsigned32);
		case 2:	return sizeof(unsigned8);
		case 3:	return sizeof(unsigned16);
		case 4:	return sizeof(unsigned8);
		case 5:	return sizeof(unsigned16);
		case 6:	return sizeof(unsigned8);
	}
	return CAN_ERRET_OBD_NOSUBIND;
}

int16 get_pdo_comm_bytes_objsize(canindex index, cansubind subind)	// 2.2.1 some changes
{
	if (check_pdo_comm_object(index) == CAN_ERRET_OBD_NOOBJECT) {
		return CAN_ERRET_OBD_NOOBJECT;
	}
	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {		// 2.3.1 new CCT
		if (subind == 6) return CAN_ERRET_OBD_NOSUBIND;
	}
	return pdo_comm_bytes_objsize(subind);
}

int16 see_pdo_comm_access(canindex index, cansubind subind)
{
	int16 size;

	size = get_pdo_comm_bytes_objsize(index, subind);
	if (size <= 0) return size;
	if (subind == 0) return CAN_MASK_ACCESS_RO;
	return CAN_MASK_ACCESS_RW;
}

int16 get_pdo_comm_objtype(canindex index, cansubind subind)
{
	int16 size;

	size = get_pdo_comm_bytes_objsize(index, subind);
	if (size <= 0) return size;
	switch (subind) {
		case 1:	return CAN_DEFTYPE_UNSIGNED32;
		case 3:	return CAN_DEFTYPE_UNSIGNED16;
		case 5:	return CAN_DEFTYPE_UNSIGNED16;
	}
	return CAN_DEFTYPE_UNSIGNED8;
}

int16 read_pdo_communication(canindex index, cansubind subind, unsigned32 *data)	// 2.2.2 some changes
{
	int16 size, entry;

	*data = 0;
	find_pdo_comm_entry(index, &entry);
	if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
	size = pdo_comm_bytes_objsize(subind);
	if (size <= 0) return size;
	if (subind == 0) {
		if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) *data = 5;
		else *data = 6;
	} else {
		*data = 0;
		if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
			if (subind == 1) *data = rcvpdo[entry].id;
			else if (subind == 2) *data = rcvpdo[entry].trtype;
			else if (subind == 5) *data = rcvpdo[entry].et;
		} else {
			if (subind == 1) *data = trnpdo[entry].id;
			else if (subind == 2) *data = trnpdo[entry].trtype;
			else if (subind == 3) *data = trnpdo[entry].inh;
			else if (subind == 5) *data = trnpdo[entry].et;
			else if (subind == 6) *data = trnpdo[entry].syncstart;
		}
	}
	return CAN_RETOK;
}

int16 write_pdo_communication(canindex index, cansubind subind, unsigned32 data)		// 2.3.2 some changes
{
	int16 size, entry, fnr;
	unsigned32 pdoid;
	unsigned8 pdovld, bufvld, rpdo;

	find_pdo_comm_entry(index, &entry);
	if (entry < 0) return CAN_ERRET_OBD_NOOBJECT;
	size = pdo_comm_bytes_objsize(subind);
	if (size <= 0) return size;
	if (subind == 0) return CAN_ERRET_OBD_READONLY;
	if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
		rpdo = TRUE;
		pdoid = rcvpdo[entry].id;
	} else {
		rpdo = FALSE;
		pdoid = trnpdo[entry].id;
	}
	if ((pdoid & CAN_MASK_COBID_INVALID) == 0) pdovld = VALID;
	else pdovld = NOT_VALID;
	if (subind == 1) {
		if ((data & CAN_MASK_COBID_INVALID) == 0) bufvld = VALID;
		else bufvld = NOT_VALID;
		if (pdovld == VALID && bufvld == VALID) return CAN_ERRET_OBD_OBJACCESS;
		if ((data & CAN_MASK_IDSIZE) != 0) return CAN_ERRET_OBD_VALRANGE;
		if (check_canid_restricted((canlink)data) == RESTRICTED) return CAN_ERRET_OBD_VALRANGE;		// 2.2.1
		data &= (MASK_RTR_ALLOWED | CAN_MASK_CANID);
		data |= CAN_MASK_COBID_INVALID;
		if (rpdo) {
			rcvpdo[entry].id = data;
			fnr = set_pdo_state(index, bufvld);
			if (fnr != CAN_RETOK) rcvpdo[entry].id = pdoid;
			rcvpdo[entry].etcnt = -1;		// 2.3.2, activation resynchronized
		} else {
			trnpdo[entry].id = data;
			fnr = set_pdo_state(index, bufvld);
			if (fnr != CAN_RETOK) trnpdo[entry].id = pdoid;
			set_pdo_tran_et(entry);			// 2.3.2, event timer resynchronized
		}
		return fnr;
	} else if (subind == 2) {
		data &= 0xFF;
		if (data >= 241 && data <= 251) return CAN_ERRET_OBD_VALRANGE;
		if (rpdo) {
			if (data >= 252 && data <= 253) return CAN_ERRET_OBD_VALRANGE;
			rcvpdo[entry].trtype = (unsigned8)data;
		} else {
			trnpdo[entry].trtype = (unsigned8)data;
			trnpdo[entry].resync = ON;
			set_pdo_tran_et(entry);		// 2.3.1, event timer depends on trtype
		}
		return CAN_RETOK;
	} else if (subind == 4) {	// 2.2.2
		return CAN_RETOK;
	} else if (subind == 5) {
		if (rpdo) {
			rcvpdo[entry].et = (unsigned16)data;
			rcvpdo[entry].etcnt = -1;		// 2.3.1, activated within the next reception of an RPDO
		} else {
			trnpdo[entry].et = (unsigned16)data;
			set_pdo_tran_et(entry);
		}
		return CAN_RETOK;
	}
	if (rpdo) return CAN_RETOK;
	if (pdovld == VALID) return CAN_ERRET_OBD_OBJACCESS;
	if (subind == 3) {
		trnpdo[entry].inh = (unsigned16)data;
	} else if (subind == 6) {
		data &= 0xFF;
		if (data >= 241) return CAN_ERRET_OBD_VALRANGE;
		trnpdo[entry].syncstart = (unsigned8)data;
		trnpdo[entry].resync = ON;
	}
	return CAN_RETOK;
}

void can_init_pdo(void)
{
	unsigned16 cnt;
	canlink canid;
	cannode node;
	unsigned16 ent;

	for (node = 1; node <= CAN_NOF_NODES; node++) {
		canid = CAN_CANID_PDO_TRN_BASE + node;
		ent = (node-1)*CAN_NOF_PDO_TRAN_SLAVE;
		for (cnt = 0; cnt < CAN_NOF_PDO_TRAN_SLAVE; cnt++) {
			rcvpdo[cnt+ent].id = canid | CAN_MASK_COBID_INVALID;
			rcvpdo[cnt+ent].trtype = CAN_RPDO_TRTYPE;
			rcvpdo[cnt+ent].et = CAN_RPDO_ET_MS;
			rcvpdo[cnt+ent].etcnt = 0;
			rcvpdo[cnt+ent].node = node;
			canid += CAN_CANID_PDO_SHIFT;
		}
		canid = CAN_CANID_PDO_RCV_BASE + node;
		ent = (node-1)*CAN_NOF_PDO_RECV_SLAVE;
		for (cnt = 0; cnt < CAN_NOF_PDO_RECV_SLAVE; cnt++) {
			trnpdo[cnt+ent].id = canid | CAN_MASK_COBID_INVALID;
			trnpdo[cnt+ent].trtype = CAN_TPDO_TRTYPE;
			trnpdo[cnt+ent].inh = CAN_TPDO_INHIBIT_100MCS;
			trnpdo[cnt+ent].et = CAN_TPDO_ET_MS;
			trnpdo[cnt+ent].syncstart = CAN_TPDO_SYNC_START;
			trnpdo[cnt+ent].resync = ON;
			trnpdo[cnt+ent].trcnt = 0;
			trnpdo[cnt+ent].inhcnt = 0;
			trnpdo[cnt+ent].etcnt = 0;
			trnpdo[cnt+ent].node = node;
			canid += CAN_CANID_PDO_SHIFT;
		}
	}
	can_init_pdo_map();
	can_init_pdo_proc();
}

#endif
