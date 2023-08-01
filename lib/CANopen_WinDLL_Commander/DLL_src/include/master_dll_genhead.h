#ifndef __DLL_GENHEAD_H__
#define __DLL_GENHEAD_H__

#include <master_dll_defines.h>
#include <__logger_dll_defines.h>
#include <__application_dll_defines.h>
#include <__obd_dll_defines.h>

#define _WIN32_WINNT 0x0601
#include <windows.h>
#ifdef _MT
#include <process.h>
#endif

#include <time.h>

#include <chai.h>

#include <master_dll_typedefs.h>
#include <master_dll_macros.h>
#include <master_dll_structures.h>
#include <master_dll_globals.h>

#endif
