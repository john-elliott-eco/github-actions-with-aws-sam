using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace ServerlessAPI.Services
{
    public class GetParams
    {
        public async Task<Dictionary<string, string>> GetParametersAsync(string parameterName, bool decrypt)
        {
            using var client = new AmazonSimpleSystemsManagementClient();
            var request = new GetParametersRequest
            {
                Names = new List<string> { parameterName },
                WithDecryption = decrypt // Set to true if the parameter is encrypted
            };

            var response = await client.GetParametersAsync(request);

            if (response.InvalidParameters.Count > 0)
            {
                throw new Exception($"Invalid parameters: {string.Join(", ", response.InvalidParameters)}");
            }

            return response.Parameters.ToDictionary(p => p.Name, p => p.Value);
        }
    }
}
