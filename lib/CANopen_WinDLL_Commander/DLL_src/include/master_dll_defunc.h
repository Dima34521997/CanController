#ifndef __DLL_DEFUNC_H__
#define __DLL_DEFUNC_H__

// System dependent functions
void can_sleep(int32 microseconds);
void can_init_system_timer(void (*handler)(void));
void can_cancel_system_timer(void);
void init_critical(void);
void enter_critical(void);
void leave_critical(void);
void enable_can_transmitter(void);
void disable_can_transmitter(void);

// ..CANopen\master_dll_client.c functions below
void can_sdo_client_transfer(struct sdocltappl *ca);

// ..CANopen\master_dll_cltrans.c functions below
void can_client_sdo(canframe *cf);
void can_client_basic(struct sdoclttrans *ct);
void can_client_control(void);
void can_init_client(void);

// ..CANopen\master_dll_canid.c functions below
int16 correct_recv_canid(canindex index, canlink canid);
canindex find_comm_recv_index(canlink canid);   // 3.0.1 API changed
unsigned8 check_sdo_canid(cansubind subind, canlink canid);   // 2.2.1
unsigned8 check_canid_restricted(canlink canid);   // 2.2.1
void can_init_recv_canids(void);

// ..CANopen\master_dll_inout.c functions below
void push_all_can_data(void);
int16 send_can_data(canframe *cf, unsigned16 priority);
void can_read_handler(canev ev);
void can_init_io(void);

// ..CANopen\master_dll_lib.c functions below
int16 check_node_id(cannode node);
int16 check_bitrate_index(unsigned8 br);
void clear_can_data(canbyte *data);
void u16_to_canframe(unsigned16 ud, canbyte *data);
unsigned16 canframe_to_u16(canbyte *data);
void u32_to_canframe(unsigned32 ud, canbyte *data);
unsigned32 canframe_to_u32(canbyte *data);

// ..CANopen\master_dll_obdsdo_client.c functions below
int16 find_sdo_client_recv_canid(canlink *canid);
int16 find_sdo_client_send_canid(canlink *canid);
int16 read_sdo_client_data(cansubind subind, unsigned32 *data);
int16 write_sdo_client_data(cansubind subind, unsigned32 data);
void can_init_sdo_client(void);

// ..CANopen\master_dll_obj_deftype.c functions below
int16 get_deftype_bytes_objsize(canindex index, cansubind subind);
int16 see_deftype_access(canindex index, cansubind subind);
int16 get_deftype_objtype(canindex index, cansubind subind);
int16 read_deftype_objdict(canindex index, cansubind subind, canbyte *data);
int16 write_deftype_objdict(canindex index, cansubind subind, canbyte *data);

// ..CANopen\master_dll_obj_sync.c functions below
int16 find_sync_recv_canid(canlink *canid);
unsigned8 sync_window_expired(void);
void sync_received(canframe *cf);
void control_sync(void);
void can_init_sync(void);

// ..CANopen/master_pdo_map.c functions below
int16 check_pdo_map_object(canindex index);		// 2.2.1
int16 get_pdo_map_bytes_objsize(canindex index, cansubind subind);
int16 see_pdo_map_access(canindex index, cansubind subind);
int16 get_pdo_map_objtype(canindex index, cansubind subind);
int16 map_pdo(canindex index, canframe *cf);
int16 activate_pdo(canindex index, canframe *cf);
void can_init_pdo_map(void);

// ..CANopen/master_pdo_obd.c functions below
int16 check_pdo_comm_object(canindex index);	// 2.2.1
int16 find_pdo_recv_trantype(canindex index, unsigned8 *trtype);
int16 find_pdo_tran_trantype(canindex index, unsigned8 *trtype);
int16 find_pdo_recv_canid(canindex index, canlink *canid);
int16 find_pdo_tran_canid(canindex index, canlink *canid);
void find_pdo_rtr_tran_index(canlink canid, canindex *index);
void set_pdo_tran_event_timer(canindex index);
void set_pdo_recv_event_timer(canindex index);
int16 test_cyclic_tpdo(canindex index, unsigned8 sc);
int16 test_tpdo_inhibit(canindex index);
void control_pdo(void);
int16 get_pdo_comm_bytes_objsize(canindex index, cansubind subind);
int16 see_pdo_comm_access(canindex index, cansubind subind);
int16 get_pdo_comm_objtype(canindex index, cansubind subind);
int16 set_pdo_state(canindex index, unsigned8 state);
void can_init_pdo(void);

// ..CANopen/master_pdo_proc.c functions below
void receive_can_pdo(canindex index, canframe *cf);
void transmit_rtr_pdo(canindex index);
void process_sync_pdo(unsigned8 sc);
int16 pdo_remote_transmit_request(canindex index);
void can_init_pdo_proc(void);

// ..CANopen\master_dll_sdo_proc.c functions below
void parse_sdo(struct cansdo *sd, canbyte *data);
int16 send_can_sdo(struct cansdo *sd);
void abort_can_sdo(struct sdoixs *si, unsigned32 abortcode);


// ..application\master_dll_events.c functions below
void consume_sync(unsigned8 sc);
void no_sync_event(void);
void consume_controller_error(canev ev);
void master_emcy(unsigned16 errorcode);
void master_emcy_index(unsigned16 errorcode, canindex index);
void consume_emcy(canframe *cf);
void no_pdo_event(canindex index);		// 2.2.1
void can_cache_overflow(void);

// ..application\master_dll_logger.c functions below
void flush_events_cache(void);
void log_event(struct eventlog *ev);
void master_event(unsigned8 cls, unsigned8 type, int16 code, int32 info);
void node_event(cannode node, unsigned8 cls, unsigned8 type, int16 code, int32 info);
void init_logger(void);

// ..application\master_dll_nmt_master.c functions below
void consume_nmt(canframe *cf);
void manage_master_ecp(void);
void init_node_status(void);

// ..application\master_dll_node_obd.c functions below
int32 node_get_manstan_objsize(cannode node, canindex index, cansubind subind);
int16 node_read_manstan_objdict(cannode node, canindex index, cansubind subind, canbyte *data);
int16 node_write_manstan_objdict(cannode node, canindex index, cansubind subind, canbyte *data);
void init_node_obd(void);

#endif
