#ifndef __DLL_DEFAPPL_H__
#define __DLL_DEFAPPL_H__

// ..CANopen\master_dll_backinit.c functions below
FUNCHEAD int16 start_can_master(unsigned8 net, unsigned8 br);
FUNCHEAD int16 stop_can_master(void);
FUNCHEAD void canopen_monitor(void);

// ..CANopen\master_dll_obj_sync.c functions below
FUNCHEAD unsigned32 read_sync_num(void);
FUNCHEAD int16 read_sync_object(canindex index, unsigned32 *data);
FUNCHEAD int16 write_sync_object(canindex index, unsigned32 data);

// ..application\master_dll_nmt_master.c functions below
FUNCHEAD void nmt_master_command(unsigned8 cs, cannode node);
FUNCHEAD int16 read_master_hbt(cannode node, unsigned16 *hbt);
FUNCHEAD int16 write_master_hbt(cannode node, unsigned16 hbt);
FUNCHEAD unsigned8 read_nmt_state(cannode node);

// ..application\master_dll_cltrans.c functions below
FUNCHEAD void set_sdo_timeout(unsigned32 microseconds);
FUNCHEAD unsigned32 get_sdo_timeout(void);

// ..application\master_dll_sdo_transfer.c functions below
FUNCHEAD int16 read_device_object_sdo(cannode node, canindex index, cansubind subind, canbyte *data, unsigned32 *datasize);
FUNCHEAD int16 write_device_object_sdo(cannode node, canindex index, cansubind subind, canbyte *data, unsigned32 datasize);

// ..CANopen/master_dll_pdo_obd.c functions below
FUNCHEAD void set_all_pdos_state(unsigned8 state);
FUNCHEAD int16 get_pdo_node(canindex index, cannode *node);
FUNCHEAD int16 put_pdo_node(canindex index, cannode node);
FUNCHEAD int16 read_pdo_communication(canindex index, cansubind subind, unsigned32 *data);
FUNCHEAD int16 write_pdo_communication(canindex index, cansubind subind, unsigned32 data);

// ..CANopen/master_dll_pdo_map.c functions below
FUNCHEAD int16 read_pdo_mapping(canindex index, cansubind subind, unsigned32 *data);
FUNCHEAD int16 write_pdo_mapping(canindex index, cansubind subind, unsigned32 data);

// ..CANopen/master_dll_pdo_proc.c functions below
FUNCHEAD int16 transmit_can_pdo(canindex index);

// ..application\master_dll_logger.c functions below
FUNCHEAD int16 read_logger_event(struct eventlog *ev);

// ..application\master_dll_node_obd.c functions below
FUNCHEAD int16 add_node_object_dictionary(cannode node, canindex index, cansubind subind, canindex type);
FUNCHEAD int16 read_node_object_dictionary(cannode node, canindex index, cansubind subind, unsigned8 *upd, union numbers *num);
FUNCHEAD int16 write_node_object_dictionary(cannode node, canindex index, cansubind subind, union numbers *num);
FUNCHEAD int16 get_node_updated_object(cannode *node, canindex *index, cansubind *subind, union numbers *num);

#endif
