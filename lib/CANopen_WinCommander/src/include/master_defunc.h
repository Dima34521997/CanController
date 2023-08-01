#ifndef __COMMANDER_DEFUNC_H__
#define __COMMANDER_DEFUNC_H__

// System dependent functions
void can_sleep(int32 microseconds);
void can_init_system_timer(void (*handler)(void));
void can_cancel_system_timer(void);
void init_critical(void);
void enter_critical(void);
void leave_critical(void);
void enable_can_transmitter(void);
void disable_can_transmitter(void);

// ..CANopen\master_backinit.c functions below
int16 start_can_master(void);
int16 stop_can_master(void);
void canopen_monitor(void);

// ..CANopen\master_client.c functions below
void can_sdo_client_transfer(struct sdocltappl *ca);

// ..CANopen\master_cltrans.c functions below
void can_client_sdo(canframe *cf);
void can_client_basic(struct sdoclttrans *ct);
void can_client_control(void);
void set_sdo_timeout(unsigned32 microseconds);
unsigned32 get_sdo_timeout(void);
void can_init_client(void);

// ..CANopen\master_canid.c functions below
unsigned8 check_sdo_canid(cansubind subind, canlink canid);
unsigned8 check_canid_restricted(canlink canid);

// ..CANopen\master_inout.c functions below
void push_all_can_data(void);
int16 send_can_data(canframe *cf);		// 3.0.1 API changed
void can_read_handler(canev ev);
void can_init_io(void);

// ..CANopen\master_lib.c functions below
int16 check_node_id(cannode node);
int16 check_bitrate_index(unsigned8 br);
void clear_can_data(canbyte *data);
void u16_to_canframe(unsigned16 ud, canbyte *data);
unsigned16 canframe_to_u16(canbyte *data);
void u32_to_canframe(unsigned32 ud, canbyte *data);
unsigned32 canframe_to_u32(canbyte *data);

// ..CANopen\master_obdsdo_client.c functions below
int16 find_sdo_client_recv_canid(canlink *canid);
int16 find_sdo_client_send_canid(canlink *canid);
int16 read_sdo_client_data(cansubind subind, unsigned32 *data);
int16 write_sdo_client_data(cansubind subind, unsigned32 data);
void can_init_sdo_client(void);

// ..CANopen\master_obj_sync.c functions below
int16 find_sync_recv_canid(canlink *canid);
int16 read_sync_object(canindex index, unsigned32 *data);
int16 write_sync_object(canindex index, unsigned32 data);
unsigned8 sync_window_expired(void);
void sync_received(canframe *cf);
void control_sync(void);
void can_init_sync(void);

// ..CANopen\master_sdo_proc.c functions below
void parse_sdo(struct cansdo *sd, canbyte *data);
int16 send_can_sdo(struct cansdo *sd);
void abort_can_sdo(struct sdoixs *si, unsigned32 abortcode);


// ..application\master_application.c functions below
void reset_can_node(cannode node);
void application_timer_routine(void);
void application_monitor_routine(void);
void start_can_network(void);
void init_defaults(void);

// ..application\master_can_nodes functions below
void configure_can_nodes(void);

// ..application\master_events.c functions below
void consume_sync(unsigned8 sc);
void no_sync_event(void);
void consume_controller_error(canev ev);
void master_emcy(unsigned16 errorcode);
void consume_emcy(canframe *cf);
void can_cache_overflow(void);

// ..application\master_nmt_commander.c functions below
void nmt_master_command(unsigned8 cs, cannode node);
void consume_nmt(canframe *cf);
void manage_master_ecp(void);

// ..application\master_pdo_process.c functions below
int16 transmit_can_pdo(cannode node, unsigned8 pn);
void receive_can_pdo(canframe *cf);
void process_sync_pdo(unsigned8 sc);
void can_init_pdo(void);

// ..application\master_sdo_transfer.c functions below
int16 read_device_object_sdo(cannode node, canindex index, cansubind subind, canbyte *data, unsigned32 datasize);
int16 write_device_object_sdo(cannode node, canindex index, cansubind subind, canbyte *data, unsigned32 datasize);


// ..confile\master_config_file.c functions below
void read_config(void);

// ..confile\master_filename.c functions below
void transform_file_name(char *fname, char *initfn);
void time_stamp_file_name(char *fname, char *initfn, time_t ts);

// ..confile\master_pac.c functions below
int16 find_token(char *dtok, char *data, unsigned16 *npos);
int16 parse_float(union numbers *num, char *data, unsigned16 dtype, unsigned16 *npos);
int16 parse_integer(union numbers *num, char *data, unsigned16 dtype, int16 base, unsigned16 *npos);
int16 parse_number(union numbers *num, char *data, unsigned16 dtype, unsigned16 *npos);
int16 parse_string(char *dest, char *src, char ignore, int16 strlen, unsigned16 *npos);
void clip_string(char *src, int16 strlen);


// ..logger\master_logfile.c functions below
void write_event_to_file(struct eventlog *ev, FILE *log);

// ..logger\master_logger.c functions below
void log_event(struct eventlog *ev);
void push_logger(void);
void master_event(unsigned8 cls, unsigned8 type, int16 code, int32 info);
void node_event(cannode node, unsigned8 cls, unsigned8 type, int16 code, int32 info);
void close_logger(void);
void init_logger(void);

#endif
