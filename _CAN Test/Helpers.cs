using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test;

public static class TypeSize<T>
{
    public readonly static int Size;
    static TypeSize()
    {
        var dm = new DynamicMethod("SizeOfType", typeof(int), new Type[] { });
        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Sizeof, typeof(T));
        il.Emit(OpCodes.Ret);
        Size = (int)dm.Invoke(null, null);
    }

    /// <summary>
    /// Provides the current address of the given element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static System.IntPtr AddressOf<T>(T t)
    //refember ReferenceTypes are references to the CLRHeader
    //where TOriginal : struct
    {
        System.TypedReference reference = __makeref(t);

        unsafe
        {
            return *(System.IntPtr*)(&reference);
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    static System.IntPtr AddressOfRef<T>(ref T t)
    //refember ReferenceTypes are references to the CLRHeader
    //where TOriginal : struct
    {
        unsafe
        {
            System.TypedReference reference = __makeref(t);

            System.TypedReference* pRef = &reference;

            return (System.IntPtr)pRef; //(&pRef)
        }

    }

}
