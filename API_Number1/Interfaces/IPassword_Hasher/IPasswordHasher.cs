namespace API_Number1.Interfaces.IPassword_Hasher
{
    public interface IPasswordHasher
    {
        /// <summary>
        ///  Responsável por gerar o hash de uma senha fornecida, juntamente com um valor de salt aleatório. 
        /// </summary>
        /// <param name="password">A senha a ser hashada.</param>
        /// <param name="salt"> Uma variável de saída para armazenar o valor de salt gerado.</param>
        /// <returns>Uma string contendo o hash da senha.</returns>
        public string HashPassword(string password, out string salt);
        /// <summary>
        /// Responsável por verificar se uma senha fornecida corresponde ao hash e salt armazenados.
        /// </summary>
        /// <param name="password">A senha a ser verificada.</param>
        /// <param name="storedHash">O hash da senha armazenado.</param>
        /// <param name="storedSalt">O valor de salt armazenado.</param>
        /// <returns>Um valor booleano indicando se a senha fornecida corresponde ao hash e salt armazenados.</returns>
        public bool VerifyPassword(string password, string storedHash, string storedSalt);
        
    }
}
