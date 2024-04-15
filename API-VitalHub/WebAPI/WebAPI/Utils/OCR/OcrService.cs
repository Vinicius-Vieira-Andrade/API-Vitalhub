using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace WebAPI.Utils.OCR
{
    public class OcrService
    {
        
            private readonly string _subscriptKey = "d89d07b32f9441a6a3ed384ae1963399";

            private readonly string _endpoint = "https://cvvvitalhubg10vini.cognitiveservices.azure.com/";
        public async Task<string> RecognizeTextAsync(Stream imageStream)
        {
            try
            {
                var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptKey))
                {
                    Endpoint = _endpoint
                };

                var ocrResult = await client.RecognizePrintedTextInStreamAsync(true, imageStream);

                return ProcessRecognitionResult(ocrResult);


            }
            catch (Exception e)
            {
                return "erro ao reconhecer esse texto" + e.Message ;
            }
        }

        private static string ProcessRecognitionResult(OcrResult result)
        {
            try
            {
                string recognizedText = "";

                foreach (var region in result.Regions)
                {
                    foreach (var lines in region.Lines)
                    {
                        foreach (var word in lines.Words)
                        {
                            recognizedText += word.Text + " ";
                        }

                        recognizedText += "\n";
                    }
                }

                return recognizedText;

                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
