#include <master_dll_header.h>

#if CHECK_VERSION_CANLIB(3, 0, 0)
//  CHECK_VERSION_CANLIB(2, 3, 1)

static struct cancache cancache[CAN_WRITECACHE_SIZE];
static int16 sem_cansend, sem_overflow, sem_sigio, flag_sigio, flag_push;

void push_all_can_data(void)
{
	unsigned16 cnt;

	CAN_CRITICAL_BEGIN
	do {
		flag_push = 0;
		sem_cansend++;
		if (sem_cansend == 0) {
			CAN_CRITICAL_END
			for (cnt = 0; cnt < CAN_WRITECACHE_SIZE; cnt++) {
				if (flag_push != 0) break;
				if (cancache[cnt].capture != 255) continue;		// 2.2.2
				if (CiWrite(can_network, &cancache[cnt].cf, 1) <= 0) break;
				cancache[cnt].capture = 0;
				cancache[cnt].busy = -1;
			}
			CAN_CRITICAL_BEGIN
		}
		sem_cansend--;
	} while (flag_push != 0);
	flag_push = 1;
	CAN_CRITICAL_END
}

int16 send_can_data(canframe *cf, unsigned16 priority)
{
	unsigned16 cnt, cabg;
	int16 wrac;

	cabg=0;
	if (priority <= CAN_PRIORITY_SDO) cabg += 4;
	else if (priority <= CAN_PRIORITY_PDO) cabg += 2;
	else if (priority <= CAN_PRIORITY_EMCY) cabg += 1;
	wrac = CAN_WRITECACHE_ATTEMPTS;
	do {
		CAN_CRITICAL_BEGIN
		sem_cansend++;
		CAN_CRITICAL_END
		for (cnt = cabg; cnt < CAN_WRITECACHE_SIZE; cnt++) {
			CAN_CRITICAL_BEGIN
			cancache[cnt].busy++;
			if (cancache[cnt].busy == 0) {
				CAN_CRITICAL_END
				if (cancache[cnt].capture == 0) {
					cancache[cnt].capture = 1;
					cancache[cnt].cf = *cf;
					cancache[cnt].capture = 255;		// 2.2.2
					CAN_CRITICAL_BEGIN
					sem_cansend--;
					CAN_CRITICAL_END
					push_all_can_data();
					return CAN_RETOK;
				}
			} else {
				cancache[cnt].busy--;
				CAN_CRITICAL_END
			}
		}
		CAN_CRITICAL_BEGIN
		sem_cansend--;
		CAN_CRITICAL_END
		if (sem_cansend != -1) break;
		push_all_can_data();
		can_sleep(CAN_WRITECACHE_DELAY);
		wrac--;
	} while (wrac > 0);
	CAN_CRITICAL_BEGIN
	sem_overflow++;
	if (sem_overflow == 0) {
		CAN_CRITICAL_END
		can_cache_overflow();
		CAN_CRITICAL_BEGIN
	}
	sem_overflow--;
	CAN_CRITICAL_END
	return CAN_ERRET_COMM_SEND;
}

static void receive_can_data(canframe *cf)
{
	canlink canid;
	canindex index;

	if ((cf->flags & CAN_FLAG_EFF) != 0) return;
	if (cf->len > CAN_DATALEN_MAXIMUM) cf->len = CAN_DATALEN_MAXIMUM;
	cf->id &= CAN_MASK_CANID;
	if ((cf->flags & CAN_FLAG_RTR) != 0) {
		find_pdo_rtr_tran_index((canlink)cf->id, &index);
		if (index >= CAN_INDEX_TRNPDO_COMM_MIN && index <= CAN_INDEX_TRNPDO_COMM_MAX) {
			transmit_rtr_pdo(index);
		}
		return;
	}
	if (cf->id >= CAN_CANID_NMT_ERROR_MIN && cf->id <= CAN_CANID_NMT_ERROR_MAX) {
		if (cf->len != CAN_DATALEN_ECP) return;		// 2.2.2
		consume_nmt(cf);
		return;
	}
	if (cf->id >= CAN_CANID_EMCY_MIN && cf->id <= CAN_CANID_EMCY_MAX) {
		if (cf->len != CAN_DATALEN_EMCY) return;	// 2.2.2
		consume_emcy(cf);
		return;
	}
	if (find_sync_recv_canid(&canid) == CAN_RETOK) {
		if (cf->id == canid) {
			sync_received(cf);
			return;
		}
	}
	if (cf->id >= CAN_CANID_PDO1_TRN_MIN && cf->id <= CAN_CANID_PDO4_RCV_MAX) {		// All IDs - may be re-configured
		index = find_comm_recv_index((canlink)cf->id);
		if (index >= CAN_INDEX_RCVPDO_COMM_MIN && index <= CAN_INDEX_RCVPDO_COMM_MAX) {
			receive_can_pdo(index, cf);
			return;
		}
	}
	if (cf->id >= CAN_CANID_SDO_SC_MIN && cf->id <= CAN_CANID_SDO_SC_MAX) {
		if (cf->len != CAN_DATALEN_SDO) return;		// 2.2.2
		if (find_sdo_client_recv_canid(&canid) == CAN_RETOK) {
			if (cf->id == canid) can_client_sdo(cf);
		}
		return;
	}
}

void can_read_handler(canev ev)
{
	canframe cf;

	CAN_CRITICAL_BEGIN
	do {
		flag_sigio = 0;
		sem_sigio++;
		if (sem_sigio == 0) {
			CAN_CRITICAL_END
			while (CiRead(can_network, &cf, 1) > 0) receive_can_data(&cf);
			CAN_CRITICAL_BEGIN
		}
		sem_sigio--;
	} while (flag_sigio != 0);
	flag_sigio = 1;
	CAN_CRITICAL_END
}

void can_init_io(void)
{
	unsigned16 cnt;

	sem_cansend = 0;
	for (cnt = 0; cnt < CAN_WRITECACHE_SIZE; cnt++) {
		cancache[cnt].capture = 0;
		cancache[cnt].busy = -1;
	}
	sem_cansend = -1;
	sem_sigio = -1;
	sem_overflow = -1;
}

#endif
