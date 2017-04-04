using System;
using System.Runtime.CompilerServices;

namespace NSec.Cryptography
{
    internal static class Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreSameStart(
            ReadOnlySpan<byte> first,
            ReadOnlySpan<byte> second)
        {
            return Unsafe.AreSame(ref first.DangerousGetPinnableReference(), ref second.DangerousGetPinnableReference());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Overlap(
            ReadOnlySpan<byte> first,
            ReadOnlySpan<byte> second)
        {
            fixed (byte* x1 = &first.DangerousGetPinnableReference())
            fixed (byte* y1 = &second.DangerousGetPinnableReference())
            {
                byte* x2 = x1 + first.Length;
                byte* y2 = y1 + second.Length;

                return (x1 < y2) && (y1 < x2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(
            this ReadOnlySpan<byte> span)
            where T : struct
        {
            if (span.Length < Unsafe.SizeOf<T>())
            {
                ThrowInternalError();
            }

            return Unsafe.ReadUnaligned<T>(ref span.DangerousGetPinnableReference());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadBigEndian(
            this ReadOnlySpan<byte> span)
        {
            return BitConverter.IsLittleEndian ? Reverse(span.Read<uint>()) : span.Read<uint>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadLittleEndian(
            this ReadOnlySpan<byte> span)
        {
            return BitConverter.IsLittleEndian ? span.Read<uint>() : Reverse(span.Read<uint>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToBigEndian(
            uint value)
        {
            return BitConverter.IsLittleEndian ? Reverse(value) : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(
            this Span<byte> span,
            T value)
            where T : struct
        {
            if (span.Length < Unsafe.SizeOf<T>())
            {
                ThrowInternalError();
            }

            Unsafe.WriteUnaligned(ref span.DangerousGetPinnableReference(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBigEndian(
            this Span<byte> span,
            uint value)
        {
            span.Write(BitConverter.IsLittleEndian ? Reverse(value) : value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLittleEndian(
            this Span<byte> span,
            uint value)
        {
            span.Write(BitConverter.IsLittleEndian ? value : Reverse(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Reverse(
            uint value)
        {
            return unchecked(
                (value << 24) |
                ((value & 0xFF00) << 8) |
                ((value & 0xFF0000) >> 8) |
                (value >> 24));
        }

        private static void ThrowInternalError()
        {
            throw Error.Cryptographic_InternalError();
        }
    }
}
