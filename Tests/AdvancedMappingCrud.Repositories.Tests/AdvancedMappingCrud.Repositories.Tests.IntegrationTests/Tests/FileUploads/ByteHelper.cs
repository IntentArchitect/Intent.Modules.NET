using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.FileUploads
{
    internal class ByteHelper
    {
        internal static byte[] ReadBytes(Stream input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[4096]; // Adjust the buffer size as needed

                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                return memoryStream.ToArray();
            }
        }

    }
}
