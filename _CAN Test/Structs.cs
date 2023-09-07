using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test;

/*
Примечания:
если бит 0 поля flags выставлен в 1 – кадр помечен как RTR
если бит 2 поля flags выставлен в 1 – кадр помечен как EFF (Extended 
Frame Format, идентификатор - 29 бит)
        
время, указываемое в поле ts, измеряется от момента открытия канала в 
микросекундах, переполнение этого поля наступает примерно через 71 
минуту после открытия канала; после переполнения время вновь 
отсчитывается от нуля.
*/

#region CHAI

public struct canmsg_t
{
    public uint id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] data;
    public byte len;
    public ushort flags;
    public uint ts;
}

internal struct canboard_t
{
    public byte brdnum;        /* номер платы (от 0 до CI_BRD_NUMS-1)*/
    public uint hwver;       /* номер версии железа (аналогичен по структуре номеру версии библиотеки */
    public short[] chip;       /* массив номеров каналов (например chip[0] содержит номер канала 
                            к которому привязан первый чип платы, если номер <0 - чип отсутствует)*/
    public string name;        /* текстовая строка названия платы */
    public string manufact;    /* текстовая строка - имя производителя */

}

internal struct canwait_t
{
    public byte chan;          // номер открытого канала
    public byte wflags;        // флаги интересующих нас событий
    public byte rflags;        // флаги наступивших событий (результат выполнения)
}

internal struct canerrs_t
{
    public ushort ewl;         // кол-во ошибок EWL
    public ushort boff;        // кол-во ошибок BOFF
    public ushort hwovr;       // кол-во ошибок HOVR
    public ushort swovr;       // кол-во ошибок SOVR
    public ushort wtout;       // кол-во ошибок WTOUT
}
#endregion

#region Codes

#region CHAI
public enum CHAICodes
{
    ECIOK = 0,      /* success */
    ECIGEN = 1,     /* generic (not specified) error */
    ECIBUSY = 2,    /* device or resourse busy */
    ECIMFAULT = 3,  /* memory fault */
    ECISTATE = 4,   /* function can't be called for chip in current state */
    ECIINCALL = 5,  /* invalid call, function can't be called for this object */
    ECIINVAL = 6,   /* invalid parameter */
    ECIACCES = 7,   /* can not access resource */
    ECINOSYS = 8,   /* function or feature not implemented */
    ECIIO = 9,      /* input/output error */
    ECINODEV = 10,  /* no such device or object */
    ECIINTR = 11,   /* call was interrupted by event */
    ECINORES = 12,  /* no resources */
    ECITOUT = 13,   /* time out occured */
}

public enum PredefinedBaudRates
{
    BCI_1M_bt0 = 0x00,
    BCI_1M_bt1 = 0x14,
    BCI_800K_bt0 = 0x00,
    BCI_800K_bt1 = 0x16,
    BCI_500K_bt0 = 0x00,
    BCI_500K_bt1 = 0x1c,
    BCI_250K_bt0 = 0x01,
    BCI_250K_bt1 = 0x1c,
    BCI_125K_bt0 = 0x03,
    BCI_125K_bt1 = 0x1c,
    BCI_100K_bt0 = 0x04,
    BCI_100K_bt1 = 0x1c,
    BCI_50K_bt0 = 0x09,
    BCI_50K_bt1 = 0x1c,
    BCI_20K_bt0 = 0x18,
    BCI_20K_bt1 = 0x1c,
    BCI_10K_bt0 = 0x31,
    BCI_10K_bt1 = 0x1c,
}

#endregion

public enum Defines
{
    GEN_ERRET = -1,
    GEN_RETOK = 0,
    CAN_ERRET = -1,
    CAN_RETOK = 0,

    CAN_RETOK_MAIN = 0,
    CAN_RETURN_MAIN = 1,
    CAN_ERRET_MAIN = 2,

    CAN_NETWORK_CONTROLLER = 1,                // Default device channel
    CAN_BITRATE_INDEX = CIA_BITRATE_INDEX_125, // Default BaudRate

    CIA_BITRATE_TABLE = 0,
    CIA_BITRATE_INDEX_1000 = 0,
    CIA_BITRATE_INDEX_800 = 1,
    CIA_BITRATE_INDEX_500 = 2,
    CIA_BITRATE_INDEX_250 = 3,
    CIA_BITRATE_INDEX_125 = 4,
    CIA_BITRATE_INDEX_50 = 6,
    CIA_BITRATE_INDEX_20 = 7,
    CIA_BITRATE_INDEX_10 = 8,
    CIA_BITRATE_INDEX_AUTO = 9,

    // *** CAN_ERRET_... - Functions return error codes - negative values only ***

    GEN_ERRET_LOGGER_BUSY = -919, // Logger is busy, try to read later
    GEN_ERRET_NO_LOGEVENT = -918, // NO logger events
    GEN_ERRET_NO_UPDATED = -911, // NO objects with updated data
    GEN_ERRET_OUTOFMEM = -910, // Out of memory - no public const int 
    GEN_ERRET_DUPLICATED = -907, // Duplicated token
    GEN_ERRET_TOKEN = -906, // Token not found
    GEN_ERRET_EMPTY = -905, // Empty parsed string
    GEN_ERRET_MEMALLOC = -904, // Memory allocation error
    GEN_ERRET_VALUE = -903, // Invalid data value
    GEN_ERRET_DATATYPE = -902, // Invalid (unsupported) data type
    GEN_ERRET_FILEOPEN = -901, // File open error
    GEN_ERRET_TOOMANY = -900, // Too many errors public const int 
    CAN_ERRET_INV_NMTSTATE = -213, // Invalid CAN node NMT state
    CAN_ERRET_INV_DEVCONFIG = -212, // Invalid device configuration
    CAN_ERRET_INV_DEVICE = -211, // Invalid CANopen device
    CAN_ERRET_NO_DEVICE = -210, // Node-id device not public const int 
    CAN_ERRET_CI_WTOUT = -139, // CAN driver timeout setting failed
    CAN_ERRET_CI_WRITE = -138, // CAN driver data write operation failed
    CAN_ERRET_CI_BITRATE = -137, // CAN controller baud rate could not be set.
    CAN_ERRET_CI_HANDLER = -136, // CAN driver handler registration failed
    CAN_ERRET_CI_FILTER = -135, // CAN controler acceptance filter set failed
    CAN_ERRET_CI_START = -134, // CAN controller transition to the start state failed
    CAN_ERRET_CI_STOP = -133, // CAN controller transition to the stop state failed
    CAN_ERRET_CI_CLOSE = -132, // CAN IO channel closing error
    CAN_ERRET_CI_OPEN = -131, // CAN IO channel opening error
    CAN_ERRET_CI_INIT = -130, // CAN CHAI init failed
    CAN_ERRET_OBD_INTINCOMP = -120, // General internal incompatibility in the device 2.2.x
    CAN_ERRET_OBD_PARINCOMP = -119, // General parameter incompatibility reason
    CAN_ERRET_OBD_DEVSTATE = -118, // Invalid present device state
    CAN_ERRET_OBD_NODATA = -117, // No data available
    CAN_ERRET_OBD_DATALOW = -116, // Data type does not match, length of service parameter too low
    CAN_ERRET_OBD_DATAHIGH = -115, // Data type does not match, length of service parameter too high
    CAN_ERRET_OBD_DATAMISM = -114, // Data type does not match, length of service parameter does not match
    CAN_ERRET_OBD_VALERR = -113, // Maximum value is less then minimum value
    CAN_ERRET_OBD_VALLOW = -112, // Value of parameter written too low
    CAN_ERRET_OBD_VALHIGH = -111, // Value of parameter written too high
    CAN_ERRET_OBD_VALRANGE = -110, // Value range of parameter exceeded (only for write access)
    CAN_ERRET_OBD_NODIRECT = -107, // Object can not be addressed directly (with its pointer)
    CAN_ERRET_OBD_OBJACCESS = -106, // Unsupported access to an object
    CAN_ERRET_OBD_WRITEONLY = -105, // Attempt to read a write only object
    CAN_ERRET_OBD_READONLY = -104, // Attempt to write a read only object
    CAN_ERRET_OBD_INVNODE = -103, // Invalid node
    CAN_ERRET_OBD_NOOBJECT = -102, // Object does not exist in the object dictionary
    CAN_ERRET_OBD_NOSUBIND = -101, // Sub-index does not exist
    CAN_ERRET_OUTOFMEM = -100, // No space [to dispose new index for receive CAN-ID] 2.2.x
    CAN_ERRET_FLASH_DATA = -69, // Invalid/inconsistent flash data
    CAN_ERRET_FLASH_VALUE = -68, // No flash parameter value
    CAN_ERRET_FLASH_INIT = -67, // Flash page initialization failed
    CAN_ERRET_FLASH_LOCKED = -66, // Write to the flash disabled
    CAN_ERRET_SIGNATURE = -61, // Wrong signature
    CAN_ERRET_RE_STORE = -60, // Default parameters save/load failed
    CAN_ERRET_COMM_SEND = -50, // An attempt to send CAN data into the network failed
    CAN_ERRET_EMCY_INHIBIT = -42, // Emergency is inhibited
    CAN_ERRET_EMCY_INVALID = -41, // Emergency is not valid
    CAN_ERRET_SYNC_NOTGENER = -40, // Device does not generate SYNC message
    CAN_ERRET_PDO_NOMAP = -32, // Object cannot be mapped to the PDO
    CAN_ERRET_PDO_ERRMAP = -31, // Количество и длина отображаемых объектов превысит длину PDO.
    CAN_ERRET_PDO_MAP_DEACT = -30, // PDO mapping deactivated
    CAN_ERRET_PDO_TRTYPE = -24, // PDO transmittion type is invalid (requested operation is not supported)
    CAN_ERRET_PDO_NORTR = -23, // RTR is not allowed for the PDO
    CAN_ERRET_PDO_INHIBIT = -22, // TPDO is inhibited
    CAN_ERRET_PDO_TRIGGER = -21, // Cyclic TPDO trigger counter has not expired yet
    CAN_ERRET_PDO_INVALID = -20, // PDO is not valid
    CAN_ERRET_SDO_INVALID = -10, // SDO is not valid
    CAN_ERRET_BITRATE = -5, // Invalid configured bitrate index
    CAN_ERRET_NODEID = -4, // Invalid configured NodeID
    CAN_ERRET_NULL_POINTER = -3, // NULL pointer to an object returned
    CAN_ERRET_NODE_STATE = -2, // Invalid node state

    // *** CAN_NMT_...  Can network management protocoles command specifiers ***

    CAN_NMT_CS_DUMMY = 0,
    CAN_NMT_START_REMOTE_NODE = 1,
    CAN_NMT_STOP_REMOTE_NODE = 2,
    CAN_NMT_ENTER_PRE_OPERATIONAL = 128,
    CAN_NMT_RESET_NODE = 129,
    CAN_NMT_RESET_COMMUNICATION = 130,

    // *** CAN_ABORT_SDO_... - CANopen SDO abort codes ***

    CAN_ABORT_SDO_TOGGLE = 0x05030000, // Toggle bit not altered
    CAN_ABORT_SDO_TIMEOUT = 0x05040000, // SDO protovol timed out
    CAN_ABORT_SDO_CS = 0x05040001, // Client/server command specifier not valid or unknown
    CAN_ABORT_SDO_BLKSIZE = 0x05040002, // Invalid block size (block mode only)
    CAN_ABORT_SDO_SEQNO = 0x05040003, // Invalid sequence number (block mode only)
    CAN_ABORT_SDO_CRC = 0x05040004, // CRC error (block mode only)
    CAN_ABORT_SDO_OUTOFMEM = 0x05040005, // Out of memory
    CAN_ABORT_SDO_OBJACCESS = 0x06010000, // Unsupported access to an object
    CAN_ABORT_SDO_WRITEONLY = 0x06010001, // Attempt to read a write only object
    CAN_ABORT_SDO_READONLY = 0x06010002, // Attempt to write a read only object
    CAN_ABORT_SDO_NOOBJECT = 0x06020000, // Object does not exist in the object dictionary
    CAN_ABORT_SDO_NOPDOMAP = 0x06040041, // Object cannot be mapped to the PDO
    CAN_ABORT_SDO_ERRPDOMAP = 0x06040042, // The number and length of the objects to be mapped would exceed PDO length
    CAN_ABORT_SDO_PARINCOMP = 0x06040043, // General parameter incompatibility reason
    CAN_ABORT_SDO_INTINCOMP = 0x06040047, // General internal incompatibility in the device
    CAN_ABORT_SDO_HARDWARE = 0x06060000, // Access failed due to a hardware error
    CAN_ABORT_SDO_DATAMISM = 0x06070010, // Data type does not match, length of service parameter does not match
    CAN_ABORT_SDO_DATAHIGH = 0x06070012, // Data type does not match, length of service parameter too high
    CAN_ABORT_SDO_DATALOW = 0x06070013, // Data type does not match, length of service parameter too low
    CAN_ABORT_SDO_NOSUBIND = 0x06090011, // Sub-index does not exist
    CAN_ABORT_SDO_VALRANGE = 0x06090030, // Value range of parameter exceeded (only for write access)
    CAN_ABORT_SDO_VALHIGH = 0x06090031, // Value of parameter written too high
    CAN_ABORT_SDO_VALLOW = 0x06090032, // Value of parameter written too low
    CAN_ABORT_SDO_VALERR = 0x06090036, // Maximum value is less then minimum value
    CAN_ABORT_SDO_ERROR = 0x08000000, // General error
    CAN_ABORT_SDO_TRAPPL = 0x08000020, // Data cannot be transferred or stored to the application
    CAN_ABORT_SDO_TRAPLC = 0x08000021, // Data cannot be transferred or stored to the application because of local control
    CAN_ABORT_SDO_TRAPDS = 0x08000022, // Data cannot be transferred or stored to the application because of the present device state
    CAN_ABORT_SDO_OBJDICT = 0x08000023, // Object dictionary dynamic generation fails or no object dictionary is present
    CAN_ABORT_SDO_NODATA = 0x08000024, // No data available

    // *** CAN_EMCY_... - CANopen emergency error codes ***

    CAN_EMCY_NO_ERROR = 0x0000,
    CAN_EMCYREG_CACHE = 0x6180,// CAN send cache overflow. Registered only in object 0x1003
    CAN_EMCYREG_TIMERFAIL = 0x6190,// System timer initiation failed. Registered only in object 0x1003.
    CAN_EMCY_TIMEROVERLAP = 0x6191,// System timer signals 
    CAN_EMCY_STORE_CRC = 0x61A0,// Non volatile memory data inconsistency (CRC error).
    CAN_EMCY_STORE_ERR = 0x61A1,// Non volatile memory data processing error. Registered only in object 0x1003
    CAN_EMCY_HEARTBEAT = 0x8130,// Life guard error or heartbeat 
    CAN_EMCY_HOVR = 0x8180,// CAN hardware overrun
    CAN_EMCY_SOVR = 0x8181,// CAN software overrun
    CAN_EMCY_EWL = 0x8182,// CAN error warning limit
    CAN_EMCY_WTOUT = 0x8183,// CAN write timeout
    CAN_EMCYREG_BOFF = 0x818F,// CAN bus off - master 
    CAN_EMCY_PDOLENERR = 0x8210,// PDO not processed due to length error
    CAN_EMCY_PDOLENEXC = 0x8220,// PDO length 
    CAN_EMCY_SYNCLEN = 0x8240,// Unexpected SYNC data length
    CAN_EMCY_RPDO_TIMEOUT = 0x8250,// RPDO timeout

    // *** CAN_NOF_... - various reserved numbers ***

    CAN_NOF_PDOBIT_MAX = 64,  // Maximum PDO length in BIT
    CAN_NOF_PDO_RECEIVE = (CAN_NOF_PDO_TRAN_SLAVE * CAN_NOF_NODES),// 2.3.0 Crossed
    CAN_NOF_PDO_TRANSMIT = (CAN_NOF_PDO_RECV_SLAVE * CAN_NOF_NODES),  // 2.3.0 Crossed
    CAN_SYNCPDO_RECEIVE = (1 + CAN_NOF_SYNCPDO_MASTER),   // 2.3.0 Synchronous RPDO FIFO size
    CAN_SYNCPDO_TRANSMIT = (1 + CAN_NOF_SYNCPDO_MASTER),   // 2.3.0 Synchronous TPDO FIFO size

    MAJ_VERS_APPL = 1, // Major application software version
    MIN_VERS_APPL = 1, // Minor application software version
    RELEASE_APPL = 0, // The application software release
    CAN_TIMERUSEC = 10000, // CANopen timer period in microseconds (10 milliseconds).
    CAN_TIMEOUT_RETRIEVE = 500000, // SDO client BASIC transaction data retrieve DEFAULT timeout - microseconds
    CAN_TIMEOUT_READ = 200000, // SDO client BASIC transaction data read timeout - microseconds

    // *** NO nested client transactions ***
    CAN_NOF_PDO_RECV_SLAVE = 4,// No of receive PDO communication parameters for the Slave
    CAN_NOF_PDO_TRAN_SLAVE = 4,// No of transmit PDO communication parameters for the public const int 
    CAN_NOF_SYNCPDO_MASTER = 64,// Synchronous RPDO and TPDO buffer sizes for the Master, 2.public const int 
    CAN_RPDO_TRTYPE = 255,// Receive PDO default transmission type
    CAN_RPDO_ET_MS = 0,// RPDO event timer default time in public const int 
    CAN_TPDO_TRTYPE = 255,// Transmit PDO default transmission type
    CAN_TPDO_INHIBIT_100MCS = 0,// TPDO inhibit default time in hundreds of microseconds
    CAN_TPDO_ET_MS = 0,// TPDO event timer default time in ms
    CAN_TPDO_SYNC_START = 0,// TPDO SYNC start value

    // *** Various static defaults ***

    CAN_NODE_ID_MIN = 1,// Minimum device node ID
    CAN_NODE_ID_MAX = 127,// Maximum device node ID
    CAN_LSS_NODEID = 255,// Non configured device node ID
    CAN_NOF_NODES = 127,
    CAN_SIZEOF_FACTOR = 8,// BIT object length calculation factor
    CAN_SIZEOF_BOOLEAN = 1,// Boolean data type size both in bits and bytes

    // *** CAN_DEFTYPE_... - CAN data type entry specification indexes ***

    CAN_DEFTYPE_BOOLEAN = 0x0001,
    CAN_DEFTYPE_INTEGER8 = 0x0002,
    CAN_DEFTYPE_INTEGER16 = 0x0003,
    CAN_DEFTYPE_INTEGER32 = 0x0004,
    CAN_DEFTYPE_UNSIGNED8 = 0x0005,
    CAN_DEFTYPE_UNSIGNED16 = 0x0006,
    CAN_DEFTYPE_UNSIGNED32 = 0x0007,
    CAN_DEFTYPE_REAL32 = 0x0008,
    CAN_DEFTYPE_VISIBLE_STRING = 0x0009,
    CAN_DEFTYPE_OCTET_STRING = 0x000A,
    CAN_DEFTYPE_UNICODE_STRING = 0x000B,
    CAN_DEFTYPE_TIME_OF_DAY = 0x000C,
    CAN_DEFTYPE_TIME_DIFFERENCE = 0x000D,
    CAN_DEFTYPE_DOMAIN = 0x000F,
    CAN_DEFTYPE_INTEGER24 = 0x0010,
    CAN_DEFTYPE_REAL64 = 0x0011,
    CAN_DEFTYPE_INTEGER40 = 0x0012,
    CAN_DEFTYPE_INTEGER48 = 0x0013,
    CAN_DEFTYPE_INTEGER56 = 0x0014,
    CAN_DEFTYPE_INTEGER64 = 0x0015,
    CAN_DEFTYPE_UNSIGNED24 = 0x0016,
    CAN_DEFTYPE_UNSIGNED40 = 0x0018,
    CAN_DEFTYPE_UNSIGNED48 = 0x0019,
    CAN_DEFTYPE_UNSIGNED56 = 0x001A,
    CAN_DEFTYPE_UNSIGNED64 = 0x001B,

    // *** CAN_INDEX_... - CAN object dictionary indexes ***

    CAN_INDEX_DUMMY = 0x0000,
    CAN_INDEX_DEFTYPE_MIN = 0x0001,// Data type min index
    CAN_INDEX_DEFTYPE_MAX = 0x001F,// Data type max index
    CAN_INDEX_COMMAREA_BEGIN = 0x1000,// Communication profile area begin
    CAN_INDEX_COMMAREA_END = 0x1FFF,// Communication profile area end
    CAN_INDEX_MANSPEC_BEGIN = 0x2000,// Manufacturer specific profile area begin
    CAN_INDEX_MANSPEC_END = 0x5FFF,// Manufacturer specific profile area end
    CAN_INDEX_STANDEV_BEGIN = 0x6000,// Standardised device profile area begin
    CAN_INDEX_STANDEV_END = 0x9FFF,// Standardised device profile area end
    CAN_INDEX_DEVICE_TYPE = 0x1000,// Device type
    CAN_INDEX_ERROR_REG = 0x1001,// Error register
    CAN_INDEX_MAN_STATUS = 0x1002,// Manufacturer status register
    CAN_INDEX_PREDEF_ERROR = 0x1003,// Pre-defined Error Field
    CAN_INDEX_SYNC_COBID = 0x1005,// COB-ID SYNC
    CAN_INDEX_SYNC_PERIOD = 0x1006,// Communication cycle period
    CAN_INDEX_SYNC_WINDOW = 0x1007,// Synchronous window length
    CAN_INDEX_MAN_DEV_NAME = 0x1008,// Manufacturer device name
    CAN_INDEX_MAN_HARD_VER = 0x1009,// Manufacturer hardware version
    CAN_INDEX_MAN_SOFT_VER = 0x100A,// Manufacturer software version
    CAN_INDEX_GUARD_TIME = 0x100C,// Guard time
    CAN_INDEX_LIFETIME_FACTOR = 0x100D,// Life time factor
    CAN_INDEX_STORE = 0x1010,// Store parameters
    CAN_INDEX_RE_STORE = 0x1011,// Restore default parameters
    CAN_INDEX_EMCY_COBID = 0x1014,// COB-ID emergency object
    CAN_INDEX_EMCY_INHIBIT = 0x1015,// Inhibit time EMCY
    CAN_INDEX_CONS_HBT = 0x1016,// Consumer heartbeat time
    CAN_INDEX_PROD_HBT = 0x1017,// Producer heartbeat time
    CAN_INDEX_IDENTITY = 0x1018,// Identity object
    CAN_INDEX_SYNC_OVERFLOW = 0x1019,// Synchronous counter overflow value
    CAN_INDEX_ERR_BEHAVIOUR = 0x1029,// Error behaviour object
    CAN_INDEX_SERVER_SDO_MIN = 0x1200,// Server SDO parameter min index
    CAN_INDEX_SERVER_SDO_DEFAULT = 0x1200,// Default server SDO parameter index
    CAN_INDEX_SERVER_SDO_MAX = 0x127F,// Server SDO parameter max index
    CAN_INDEX_CLIENT_SDO_MIN = 0x1280,// Client SDO parameter min index
    CAN_INDEX_CLIENT_SDO_DEFAULT = 0x1280,// Default client SDO parameter index
    CAN_INDEX_CLIENT_SDO_MAX = 0x12FF,// Client SDO parameter max index
    CAN_INDEX_RCVPDO_COMM_MIN = 0x1400,// Receive PDO communication parameter min index
    CAN_INDEX_RCVPDO_COMM_MAX = 0x15FF,// Receive PDO communication parameter max index
    CAN_INDEX_RCVPDO_MAP_MIN = 0x1600,// Receive PDO mapping parameter min index
    CAN_INDEX_RCVPDO_MAP_MAX = 0x17FF,// Receive PDO mapping parameter max index
    CAN_INDEX_TRNPDO_COMM_MIN = 0x1800,// Transmit PDO communication parameter min index
    CAN_INDEX_TRNPDO_COMM_MAX = 0x19FF,// Transmit PDO communication parameter max index
    CAN_INDEX_TRNPDO_MAP_MIN = 0x1A00,// Transmit PDO mapping parameter min index
    CAN_INDEX_TRNPDO_MAP_MAX = 0x1BFF,// Transmit PDO mapping parameter max index

    OFF = 0,
    ON = 1,
    NOT_VALID = 0,
    VALID = 1,
    UN_RESTRICTED = 0,
    RESTRICTED = 1,
    CANID11 = 0,
    CANID29 = 1,
    DYNAMIC = 0,
    STATIC = 1,
    AFSINGLE = 0,
    AFDUAL = 1,
    NORMAL = 0,
    REVERSE = 1,
    SEPARATE = 0,
    COMBINED = 1,
    BYTES = 0,// Object size units flag
    BITS = 1,
    MAPBIT = 0,// PDO mapping granularity flag - one bit granularity
    MAPBYTE = 1,// PDO mapping granularity flag - 8 bits granularity
    SERVER = 0,// Server node operational mode
    CLIENT = 1,// Client node operational mode
    SLAVE = 0,// Slave node operational mode
    MASTER = 1,// Master node operational mode
    SIGNAL = 0,
    POLL = 1,
    RESOLVED = 0,
    OCCURRED = 1,
    CRCTABLE = 0,
    CRCDIRECT = 1,
    LSS_OFF = 0,
    LSS_WAITING = 1,
    LSS_CONFIGURATION = 2,

    // *** CAN_TRANSTATE_... - Status codes (states) for client transaction operations ***

    CAN_TRANSTATE_OBD_ZERO = -105,  // Object dictionary entry is of zero size
    CAN_TRANSTATE_OBD_READ = -104,  // Object dictionary read error
    CAN_TRANSTATE_OBD_WRITE = -103, // Object dictionary write error
    CAN_TRANSTATE_OBD_NOOBJECT = -102,  // Object does not exist in the object dictionary
    CAN_TRANSTATE_OBD_NOSUBIND = -101,  // Sub-index does not exist
    CAN_TRANSTATE_OBD_MALLOC = -100,    // Object transfer buffer allocation error
    CAN_TRANSTATE_SDO_RETRANSMIT = -44, // No of retransmit events exceeded (block protocol)
    CAN_TRANSTATE_SDO_BLKSIZE = -43,    // SDO number of segments per block invalid (block protocol)
    CAN_TRANSTATE_SDO_SEQNO = -42,  // SDO sequence number invalid (block protocol)
    CAN_TRANSTATE_SDO_CRC = -41,    // SDO CRC error (block protocol)
    CAN_TRANSTATE_SDO_SUB = -40,    // SDO subcommand invalid or mismatch (block protocol)
    CAN_TRANSTATE_SDO_TOGGLE = -39, // SDO segmented transfer toggle error
    CAN_TRANSTATE_SDO_DATASIZE = -33,   // SDO protocol data size parameter is incorrect
    CAN_TRANSTATE_SDO_OBJSIZE = -32,    // SDO object sizes, known to client and server mismatch
    CAN_TRANSTATE_SDO_MODE = -31,   // SDO client and server transfer modes mismatch
    CAN_TRANSTATE_SDO_MPX = -30,    // SDO client and server multiplexors mismatch
    CAN_TRANSTATE_SDO_SRVABORT = -20,   // SDO transfer protovol aborted by server
    CAN_TRANSTATE_SDO_INVALID = -16,    // SDO is not valid
    CAN_TRANSTATE_SDO_WRITERR = -15,    // SDO write error
    CAN_TRANSTATE_SDO_SCSERR = -14, // SDO client received not valid or unknown server command specifier
    CAN_TRANSTATE_SDO_TRANS_TIMEOUT = -13,  // SDO client BASIC transaction wait cycle timeout
    CAN_TRANSTATE_SDO_NET_TIMEOUT = -12,    // SDO client BASIC transaction network operation timeout
    CAN_TRANSTATE_SDO_READ_TIMEOUT = -11,   // SDO client BASIC transaction reset due to data read timeout
    CAN_TRANSTATE_SDO_NOWORKB = -10,    // SDO client BASIC transaction work buffer is full and unable to store new data
    CAN_TRANSTATE_SDO_NODE = -2,    // Invalid node-id of the SDO server
    CAN_TRANSTATE_ERROR = -1,   // General client transaction error
    CAN_TRANSTATE_OK = 0,		//, Succesfull transaction termination (and status after reset)
    CAN_TRANSTATE_SDO_WORK = 1		//, SDO client BASIC transaction is going on
}
#endregion