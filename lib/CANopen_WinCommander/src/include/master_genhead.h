#ifndef __MASTER_GENHEAD_H__
#define __MASTER_GENHEAD_H__

#include <master_defines.h>
#include <config_defines.h>
#include <__application_defines.h>
#include <__logger_defines.h>

#define _WIN32_WINNT 0x0601
#include <windows.h>
#ifdef _MT
#include <process.h>
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <stdarg.h>

#include <time.h>

#include <chai.h>

#include <master_typedefs.h>
#include <master_macros.h>
#include <master_structures.h>
#include <master_globals.h>

#endif
