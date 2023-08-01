#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 0)

static void node_config_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "Configure node: ");
	if (ev->code == EVENT_CODE_NODE_CONFIGURED) {
		fprintf(log, "OK");
	} else if (ev->code == EVENT_CODE_DEVICE_TYPE) {
		fprintf(log, "invalid device type 0x%08X", (unsigned32)ev->info);
	} else if (ev->code == EVENT_CODE_VENDOR_ID) {
		fprintf(log, "invalid vendor id 0x%08X", (unsigned32)ev->info);
	} else if (ev->code == EVENT_CODE_PRODUCT_CODE) {
		fprintf(log, "invalid product code 0x%08X", (unsigned32)ev->info);
	} else if (ev->code == EVENT_CODE_REVISION) {
		fprintf(log, "invalid revision number 0x%08X", (unsigned32)ev->info);
	} else if (ev->code == EVENT_CODE_SERIAL) {
		fprintf(log, "invalid serial number %-1i", (unsigned32)ev->info);
	} else if (ev->code == EVENT_CODE_PROD_HBT	) {
		fprintf(log, "heartbeat configuration failed");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}

static void node_state_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "Node state: ");
	if (ev->code == EVENT_CODE_NODE_HEARTBEAT) {
		fprintf(log, "Operational Heartbeat event");
	} else if (ev->code == EVENT_CODE_NODE_OPERATIONAL) {
		fprintf(log, "Operational");
	} else if (ev->code == EVENT_CODE_NODE_RESET) {
		fprintf(log, "Resetting node");
	} else if (ev->code == EVENT_CODE_NODE_BOOTUP) {
		fprintf(log, "BootUp event");
	} else if (ev->code == EVENT_CODE_NODE_UNDEFINED) {
		fprintf(log, "BootUp event from the UN-defined node");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}

static void node_sdo_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "SDO: ");
	if (ev->code == CAN_TRANSTATE_SDO_SRVABORT) {
		fprintf(log, "abort code 0x%08X received", (unsigned32)ev->info);
	} else if (ev->code == CAN_TRANSTATE_SDO_TOGGLE) {
		fprintf(log, "segmented transfer toggle error");
	} else if (ev->code == CAN_TRANSTATE_SDO_DATASIZE) {
		fprintf(log, "data size parameter is incorrect");
	} else if (ev->code == CAN_TRANSTATE_SDO_MPX) {
		fprintf(log, "client and server multiplexors mismatch");
	} else if (ev->code == CAN_TRANSTATE_SDO_WRITERR) {
		fprintf(log, "data write error");
	} else if (ev->code == CAN_TRANSTATE_SDO_SCSERR) {
		fprintf(log, "not valid or unknown server command specifier");
	} else if (ev->code == CAN_TRANSTATE_SDO_NET_TIMEOUT) {
		fprintf(log, "basic transaction network timeout");
	} else if (ev->code == CAN_TRANSTATE_SDO_READ_TIMEOUT) {
		fprintf(log, "basic transaction read timeout");
	} else if (ev->code == CAN_TRANSTATE_SDO_NOWORKB) {
		fprintf(log, "basic transaction work buffer is full");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}

static void master_status_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "Status:  ");
	if (ev->code == EVENT_CODE_MASTER_RUNNING) {
		fprintf(log, "running");
	} else if (ev->code == EVENT_CODE_INVALID_CONFIG) {
		fprintf(log, "invalid master configuration");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}

static void master_config_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "Config file: ");
	if (ev->code == EVENT_CODE_FILE_OPEN) {
		fprintf(log, "open error - defaults used");
	} else if (ev->code == CFG_WARNING_INV_SECTION) {
		fprintf(log, "invalid section name - ignored");
	} else if (ev->code == CFG_WARNING_INV_TOKEN) {
		fprintf(log, "invalid token - ignored");
	} else if (ev->code == CFG_WARNING_DUPL_TOKEN) {
		fprintf(log, "duplicated token - ignored");
	} else if (ev->code == CFG_WARNING_INV_VALUE) {
		fprintf(log, "invalid value - ignored");
	} else if (ev->code == CFG_WARNING_INV_NODE) {
		fprintf(log, "invalid CAN node-id - section discarded");
	} else if (ev->code == CFG_WARNING_DUPL_NODE) {
		fprintf(log, "duplicated CAN node-id - section discarded");
	} else if (ev->code == CFG_WARNING_UNDEF_NODE) {
		fprintf(log, "CAN node-id NOT defined yet - section discarded");
	} else if (ev->code == CFG_WARNING_DUPL_FILENAMES) {
		fprintf(log, "duplicated [Filenames] section - discarded");
	} else if (ev->code == CFG_WARNING_DUPL_CANNETWORK) {
		fprintf(log, "duplicated [CANnetwork] section - discarded");
	} else if (ev->code == CFG_ERROR_UNDEF_FILENAMES) {
		fprintf(log, "[Filenames] section not defined - defaults used");
	} else if (ev->code == CFG_ERROR_UNDEF_CANNETWORK) {
		fprintf(log, "[CANnetwork] section not defined - defaults used");
	} else if (ev->code == CFG_ERROR_UNDEF_LOGFILE) {
		fprintf(log, "LogFile not defined - default used");
	} else if (ev->code == CFG_ERROR_UNDEF_NETWORK) {
		fprintf(log, "CAN network not defined - default used");
	} else if (ev->code == CFG_ERROR_UNDEF_BITRATE) {
		fprintf(log, "BitRate index not defined - default used");
	} else if (ev->code == CFG_ERROR_VERSION) {
		fprintf(log, "invalid version, must be %08X - defaults used", MASTER_CONFIG_FILE_VERSION);
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	if (ev->info != EVENT_INFO_DUMMY) fprintf(log, ", line %-1i", ev->info);
	fprintf(log, "\n");
}

static void master_logger_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "Logger: ");
	if (ev->code == EVENT_CODE_CACHE_OVERFLOW) {
		fprintf(log, "cache overflow");
	} else if (ev->code == EVENT_CODE_FIFO_OVERFLOW) {
		fprintf(log, "FIFO overflow");
	} else if (ev->code == EVENT_CODE_LOGGER_FOPEN) {
		fprintf(log, "Logger create file error, default name used");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}

static void master_chai_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "CHAI: ");
	if (ev->code == CAN_ERRET_CI_BITRATE) {
		fprintf(log, "CAN controller baud rate could not be set");
	} else if (ev->code == CAN_ERRET_CI_HANDLER) {
		fprintf(log, "CAN driver handler registration failed");
	} else if (ev->code == CAN_ERRET_CI_FILTER) {
		fprintf(log, "CAN controller acceptance filter set failed");
	} else if (ev->code == CAN_ERRET_CI_START) {
		fprintf(log, "CAN controller transition to the start state failed");
	} else if (ev->code == CAN_ERRET_CI_STOP) {
		fprintf(log, "CAN controller transition to the stop state failed");
	} else if (ev->code == CAN_ERRET_CI_OPEN) {
		fprintf(log, "CAN IO channel opening error");
	} else if (ev->code == CAN_ERRET_CI_CLOSE) {
		fprintf(log, "CAN IO channel closing error");
	} else if (ev->code == CAN_ERRET_CI_INIT) {
		fprintf(log, "CAN CHAI init failed");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}

static void master_timer_event(struct eventlog *ev, FILE *log)
{
	fprintf(log, "Master timer: ");
	if (ev->code == EVENT_CODE_LOW_PERIOD) {
		fprintf(log, "period is too low");
	} else if (ev->code == EVENT_CODE_NO_CREATE) {
		fprintf(log, "failed to create waitable timer");
	} else if (ev->code == EVENT_CODE_NO_SET) {
		fprintf(log, "failed to set waitable timer");
	} else {
		fprintf(log, "error %-1i", ev->code);
	}
	fprintf(log, "\n");
}


void write_event_to_file(struct eventlog *ev, FILE *log)
{
	char ts[STR_TS_SIZE];

	if (log == NULL) return;
	if (ev->cls == EVENT_CLASS_DUMMY) return;

	if (ev->type == EVENT_TYPE_INFO)         fprintf(log, "     Info");
	else if (ev->type == EVENT_TYPE_WARNING) fprintf(log, "* Warning");
	else if (ev->type == EVENT_TYPE_ERROR)   fprintf(log, "**  ERROR");
	else if (ev->type == EVENT_TYPE_FATAL)   fprintf(log, "*** FATAL");
	else if (ev->type == EVENT_TYPE_DEBUG)   fprintf(log, "    Debug");
	else                                     fprintf(log, "? Unknown");
	fprintf(log, "   ");

	strftime(ts, STR_TS_SIZE, "%d-%m-%Y %H:%M:%S", localtime(&ev->ts));
	fprintf(log, ts);
	fprintf(log, "   ");

	if (ev->node == EVENT_NODE_MASTER) fprintf(log, "Master  ");
	else fprintf(log, "Node %-3i", ev->node);
	fprintf(log, "   ");

	if (ev->cls == EVENT_CLASS_EMCY) {
		fprintf(log, "Emergency 0x%04X\n", (unsigned16)ev->code);
		return;
	}
	if (ev->cls == EVENT_CLASS_SYNC) {
		if (ev->code == EVENT_CODE_NO_SYNC) {
			fprintf(log, "Sync NOT received\n");
			return;
		}
	}

	if (ev->cls == EVENT_CLASS_NODE_STATE) {
		node_state_event(ev, log);
		return;
	}
	if (ev->cls == EVENT_CLASS_NODE_CONFIG) {
		node_config_event(ev, log);
		return;
	}
	if (ev->cls == EVENT_CLASS_NODE_SDO) {
		node_sdo_event(ev, log);
		return;
	}

	if (ev->cls == EVENT_CLASS_MASTER_STATUS) {
		master_status_event(ev, log);
		return;
	}
	if (ev->cls == EVENT_CLASS_MASTER_CONFIG) {
		master_config_event(ev, log);
		return;
	}
	if (ev->cls == EVENT_CLASS_MASTER_LOGGER) {
		master_logger_event(ev, log);
		return;
	}
	if (ev->cls == EVENT_CLASS_MASTER_CHAI) {
		master_chai_event(ev, log);
		return;
	}
	if (ev->cls == EVENT_CLASS_MASTER_TIMER) {
		master_timer_event(ev, log);
		return;
	}

	fprintf(log, "Class %-3i   Event %-6i\n", ev->cls, ev->code);
}

#endif
