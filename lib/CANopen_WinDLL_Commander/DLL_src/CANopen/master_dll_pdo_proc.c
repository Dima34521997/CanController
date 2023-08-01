#include <master_dll_header.h>

#if CHECK_VERSION_CANLIB(3, 0, 0)
//  CHECK_VERSION_CANLIB(2, 3, 2)

static struct {
	canindex index;		// Sync RPDO index
	canframe cf;
} rpdo[CAN_SYNCPDO_RECEIVE];	// 2.3.1

static struct {
	canindex index;		// Sync TPDO index
	unsigned8 cycle;	// Sync cycle counter
} tpdo[CAN_SYNCPDO_TRANSMIT];	// 2.3.1

static unsigned16 head_rpdo, tail_rpdo;
static unsigned16 head_tpdo, tail_tpdo;
static unsigned8 syncyc;

static int16 send_can_pdo(canindex index)
{
	int16 fnr;
	canlink canid;
	canframe cf;

	fnr = test_tpdo_inhibit(index);
	if (fnr != CAN_RETOK) return fnr;
	fnr = find_pdo_tran_canid(index, &canid);
	if (fnr != CAN_RETOK) return fnr;
	CLEAR_FRAME(&cf);
	fnr = map_pdo(index, &cf);
	if (fnr != CAN_RETOK) return fnr;
	cf.id = canid;
	fnr = send_can_data(&cf, CAN_PRIORITY_PDO);
	if (fnr != CAN_RETOK) return fnr;
	set_pdo_tran_event_timer(index);
	return CAN_RETOK;
}

static int16 sync_tpdo_fifo(canindex index)
{
	unsigned16 h;

	CAN_CRITICAL_BEGIN		// 2.3.1 OS related applications
	h = head_tpdo + 1;
	if (h >= CAN_SYNCPDO_TRANSMIT) h = 0;
	if (h != tail_tpdo) {
		tpdo[h].index = index;
		tpdo[h].cycle = syncyc;
		head_tpdo = h;
		CAN_CRITICAL_END
		return CAN_RETOK;
	}
	CAN_CRITICAL_END
	return CAN_ERRET_OUTOFMEM;
}

int16 pdo_remote_transmit_request(canindex index)
{
	int16 fnr;
	canlink canid;
	canframe cf;

	fnr = find_pdo_recv_canid(index, &canid);
	if (fnr != CAN_RETOK) return fnr;
	CLEAR_FRAME(&cf);
	fnr = map_pdo(index, &cf);
	if (fnr != CAN_RETOK) return fnr;
	cf.id = canid;
	cf.flags = CAN_FLAG_RTR;
	return send_can_data(&cf, CAN_PRIORITY_PDO);
}

void receive_can_pdo(canindex index, canframe *cf)
{
	unsigned16 h;
	unsigned8 trtype;

	if (find_pdo_recv_trantype(index, &trtype) != CAN_RETOK) return;
	if (trtype >= 254) {
		activate_pdo(index, cf);
		return;
	}
	if (trtype >= 241) return;		// 2.3.1 extra check
	if (sync_window_expired() == TRUE) return;
	h = head_rpdo + 1;
	if (h >= CAN_SYNCPDO_RECEIVE) h = 0;
	if (h != tail_rpdo) {
		rpdo[h].index = index;
		rpdo[h].cf = *cf;
		head_rpdo = h;
	}	
}

int16 transmit_can_pdo(canindex index)
{
	int16 fnr;
	unsigned8 trtype;

	fnr = find_pdo_tran_trantype(index, &trtype);
	if (fnr != CAN_RETOK) return fnr;
	if (trtype == 0) {
		return sync_tpdo_fifo(index);	// Compromise: FIFO NOT re-entrant, data loss possible.
	} else if (trtype >= 254) {
		return send_can_pdo(index);
	}
	return CAN_ERRET_PDO_TRTYPE;
}

void transmit_rtr_pdo(canindex index)
{
	unsigned8 trtype;

	if (find_pdo_tran_trantype(index, &trtype) != CAN_RETOK) return;
	if (trtype >= 253) send_can_pdo(index);
	else sync_tpdo_fifo(index);		// Compromise: FIFO NOT re-entrant, data loss possible.
}

void process_sync_pdo(unsigned8 sc)
{
	unsigned16 t;
	canindex ind;

	syncyc++;
	while (tail_rpdo != head_rpdo) {
		t = tail_rpdo + 1;
		if (t >= CAN_SYNCPDO_RECEIVE) t = 0;
		if (sync_window_expired() == FALSE) activate_pdo(rpdo[t].index, &rpdo[t].cf);
		tail_rpdo = t;
	}
	while (tail_tpdo != head_tpdo) {
		t = tail_tpdo + 1;
		if (t >= CAN_SYNCPDO_TRANSMIT) t = 0;
		if (tpdo[t].cycle == syncyc) break;
		if (sync_window_expired() == FALSE) send_can_pdo(tpdo[t].index);
		tail_tpdo = t;
	}
	for (ind = CAN_INDEX_TRNPDO_COMM_MIN; ind < CAN_INDEX_TRNPDO_COMM_MIN+CAN_NOF_PDO_TRANSMIT; ind++) {
		if (test_cyclic_tpdo(ind, sc) == CAN_RETOK) {
			if (sync_window_expired() == FALSE) send_can_pdo(ind);
 		}
	}
}

void can_init_pdo_proc(void)
{
	head_rpdo = 0;
	tail_rpdo = 0;
	head_tpdo = 0;
	tail_tpdo = 0;
	syncyc = 0;
}

#endif
