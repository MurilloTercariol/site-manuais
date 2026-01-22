namespace site_manuais.Models
{
    public class Categoria
    {
        // Primary key
        public int Id { get; set; }

        // Nome da Categoria
        public string Nome { get; set; } = string.Empty;

        // Descrição Opcional da Categoria
        public string? Descricao { get; set; }

        // Data da Criação
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataUltimaAlteracao { get; set; }

        // Relacionamento categoria 1 para n módulos
        public ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();
    }
}
