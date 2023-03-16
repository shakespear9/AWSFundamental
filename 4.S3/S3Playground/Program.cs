using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

var s3Client = new AmazonS3Client();

var getObjectResult = new GetObjectRequest
{
    BucketName = "leenawscourse",
    Key = "files/movies.csv",
};

var response = await s3Client.GetObjectAsync(getObjectResult);

using var memoryStream = new MemoryStream();
response.ResponseStream.CopyTo(memoryStream);

var text = Encoding.Default.GetString(memoryStream.ToArray());

Console.WriteLine(text);














////using var inputStream = new FileStream("./font.png", FileMode.Open, FileAccess.Read);
//using var inputStream = new FileStream("./movies.csv", FileMode.Open, FileAccess.Read);

//var putObjectRequest = new PutObjectRequest
//{
//    BucketName = "leenawscourse",
//    Key = "files/movies.csv",
//    ContentType = "text/csv",
//    InputStream = inputStream
//};

//await s3Client.PutObjectAsync(putObjectRequest);






