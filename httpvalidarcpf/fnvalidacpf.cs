using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace httpvalidarcpf
{
    public static class fnvalidacpf
    {
        [FunctionName("fnvalidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string cpf =  data?.cpf;
            if(data == null){
                return new BadRequestObjectResult("Informe um cpf");
            }

            string responseMessage = $"O CPF: , {cpf}. é: " + (IsValid(cpf) == true ? "Valido": "Invalido") ;

            return new OkObjectResult(responseMessage);
        }

 public static bool IsValid(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;

        // Valida o primeiro dígito verificador
        int[] multiplicador1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma1 = 0;
        for (int i = 0; i < 9; i++)
        {
            soma1 += int.Parse(cpf[i].ToString()) * multiplicador1[i];
        }
        int resto1 = soma1 % 11;
        int digito1 = resto1 < 2 ? 0 : 11 - resto1;

        // Valida o segundo dígito verificador
        int[] multiplicador2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma2 = 0;
        for (int i = 0; i < 10; i++)
        {
            soma2 += int.Parse(cpf[i].ToString()) * multiplicador2[i];
        }
        int resto2 = soma2 % 11;
        int digito2 = resto2 < 2 ? 0 : 11 - resto2;

        // Compara os dígitos verificadores
        return cpf[9].ToString() == digito1.ToString() && cpf[10].ToString() == digito2.ToString();
    }

    }




    
}
