using System;
using System.Text;
using DotNetty.Buffers;

namespace Kengic.Infrastructure.Protocol.Rds.Commons.Converters
{
    public static class StringConverter
    {
        public static string ReadStringWithTrimEnd(this IByteBuffer byteBuffer, int length)
        {
            if (byteBuffer == null)
            {
                throw new ArgumentNullException(nameof(byteBuffer));
            }
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var resultStringNoTrimEnd = Encoding.ASCII.GetString(byteBuffer.Array,
                                                                 byteBuffer.ArrayOffset + byteBuffer.ReaderIndex,
                                                                 length);
            byteBuffer.SetReaderIndex(byteBuffer.ReaderIndex + length);
            var resultString = resultStringNoTrimEnd.TrimEnd();
            return resultString;
        }

        public static IByteBuffer WriteStringWithPadRight(this IByteBuffer byteBuffer, string stringObject, int length)
        {
            if (byteBuffer == null)
            {
                throw new ArgumentNullException(nameof(byteBuffer));
            }
            if (stringObject == null)
            {
                throw new ArgumentNullException(nameof(stringObject));
            }
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var charArray = stringObject.PadRight(length, ' ');
            var tempByteBuffer = Unpooled.WrappedBuffer(Encoding.ASCII.GetBytes(charArray));
            return byteBuffer.WriteBytes(tempByteBuffer);
        }

        public static string ReadString(this IByteBuffer byteBuffer, int length)
        {
            if (byteBuffer == null)
            {
                throw new ArgumentNullException(nameof(byteBuffer));
            }
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var resultStringNoTrimEnd = Encoding.ASCII.GetString(byteBuffer.Array,
                                                                 byteBuffer.ArrayOffset + byteBuffer.ReaderIndex,
                                                                 length);
            byteBuffer.SetReaderIndex(byteBuffer.ReaderIndex + length);
            var resultString = resultStringNoTrimEnd;
            return resultString;
        }

        public static IByteBuffer WriteString(this IByteBuffer byteBuffer, string stringObject, int length)
        {
            if (byteBuffer == null)
            {
                throw new ArgumentNullException(nameof(byteBuffer));
            }
            if (stringObject == null)
            {
                throw new ArgumentNullException(nameof(stringObject));
            }
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var charArray = stringObject;
            var tempByteBuffer = Unpooled.WrappedBuffer(Encoding.ASCII.GetBytes(charArray));
            return byteBuffer.WriteBytes(tempByteBuffer);
        }
    }
}
