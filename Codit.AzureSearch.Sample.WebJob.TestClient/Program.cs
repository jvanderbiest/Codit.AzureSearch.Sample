using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Codit.AzureSearch.Sample.WebJob.TestClient
{
    class Program
    {
        private static int _count;

        private static CloudStorageAccount StorageAccount
        {
            get
            {
                return CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnection"));
            }
        }

        static void Main()
        {
            Console.WriteLine("type 'c' to clear the blob container.");
            Console.WriteLine("type 's' to send a blob to azure storage.");
            Console.WriteLine("type 'q' to quit.");

            var blobContainerName = ConfigurationManager.AppSettings.Get("BlobContainerName");

            while (_count < 5)
            {
                var currentKey = Console.ReadKey();
                switch (currentKey.Key)
                {
                    case ConsoleKey.C:
                        RemoveBlobs(blobContainerName);
                        Console.WriteLine("\n> blobs cleared from {0}", blobContainerName);
                        break;
                    case ConsoleKey.S:
                        _count++;
                        PutOnBlob(blobContainerName,
                                   Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Format(@"sampleFiles\product{0}.xml", _count))
                                 );
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine("\n> goodbye");
                        Console.Read();
                        return;
                }
            }

            Console.WriteLine("\n> 5 blobs sent, goodbye");
            Console.Read();
        }

        static void RemoveBlobs(string blobContainerPath)
        {
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var blobContainerFiles = blobClient.GetContainerReference(blobContainerPath).ListBlobs(useFlatBlobListing: true);

            foreach (var blob in blobContainerFiles)
            {
                ((CloudBlockBlob)blob).Delete();
            }
        }

        static void PutOnBlob(string blobContainerName, string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(blobContainerName);
            blobContainer.CreateIfNotExists();

            var blob = blobContainer.GetBlockBlobReference(fileName);

            var bytes = Encoding.UTF8.GetBytes(File.ReadAllText(filePath)); ;
            blob.UploadFromByteArray(bytes, 0, bytes.Length);

            Console.WriteLine("\n> put blob {0} done", fileName);
        }
    }
}
