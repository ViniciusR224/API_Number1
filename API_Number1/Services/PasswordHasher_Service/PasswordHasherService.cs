using API_Number1.Interfaces.IPassword_Hasher;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace API_Number1.Services.PasswordHasher_Service
{
    public class PasswordHasherService : IPasswordHasher
    {
        public string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = GenerateSalt();
            salt = Convert.ToBase64String(saltBytes);
            byte[] hashBytes = GenerateHash(Encoding.UTF8.GetBytes(password), saltBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            //Teoricamente se você passou um password correto, o resultado abaixo teria o mesmo hash do
            //GenerateHash já feito na hora em que você foi fazer o registro
            byte[] computedHashBytes = GenerateHash(Encoding.UTF8.GetBytes(password), saltBytes);
            
            return hashBytes.SequenceEqual(computedHashBytes);
        }
        /// <summary>
        /// RandomNumberGenerator.Create(): Cria uma nova instância de um gerador de números aleatórios.
        /// var rng = RandomNumberGenerator.Create(): Cria uma variável rng que armazena a instância do gerador de números aleatórios.
        /// byte[] saltBytes = new byte[16]: Cria um novo array de bytes chamado saltBytes com tamanho 16.
        /// rng.GetBytes(saltBytes): Invoca o método GetBytes() do RandomNumberGenerator para preencher o array saltBytes com bytes aleatórios.
        /// </summary>
        /// <returns>Retorna o array saltBytes, que contém os bytes aleatórios gerados.</returns>
        private byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                //2-AQUI TAMBÉM
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return saltBytes;
            }
        }
        /// <summary>
        /// Cria uma instância de Rfc2898DeriveBytes, que implementa o algoritmo PBKDF2, utilizando a senha e o valor de sal fornecidos.
        /// Define o número de iterações do algoritmo PBKDF2 como 10000.
        /// Invoca o método GetBytes da instância pbkdf2 para gerar o hash da senha.
        /// </summary>
        /// <param name="passwordBytes">Os bytes da senha a serem hashados.</param>
        /// <param name="saltBytes"> Os bytes de sal utilizados na geração do hash. </param>
        /// <returns>Retorna o hash gerado como um array de bytes.</returns>
        private byte[] GenerateHash(byte[] passwordBytes, byte[] saltBytes)
        {
            //1-PROCURAR AMANHÃ ENTENDER MELHOR ESSA QUESTÃO DE CODIFICAÇÃO DE BYTES
            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000))
            {
                return pbkdf2.GetBytes(32); // 32 bytes = 256 bits
            }
        }
    }
}
