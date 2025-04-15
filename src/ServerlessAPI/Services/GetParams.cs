using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace ServerlessAPI.Services
{
    public class GetParams
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;

        public GetParams()
        {
            _ssmClient = new AmazonSimpleSystemsManagementClient();
        }

        public async Task<Dictionary<string, string>> GetParametersAsync(string path, bool withDecryption)
        {
            var request = new GetParametersByPathRequest
            {
                Path = path,
                Recursive = true,
                WithDecryption = withDecryption
            };

            var response = await _ssmClient.GetParametersByPathAsync(request);

            return response.Parameters.ToDictionary(p => p.Name, p => p.Value);
        }

        public async Task<string> GetParameterAsync(string name, bool withDecryption)
        {
            var request = new GetParameterRequest
            {
                Name = name,
                WithDecryption = withDecryption
            };

            var response = await _ssmClient.GetParameterAsync(request);
            return response.Parameter.Value;
        }

        /*
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
        */
    }
}
