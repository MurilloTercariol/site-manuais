using System.ComponentModel.DataAnnotations;

namespace site_manuais.Models
{
    public class Modulo
    {
        // Primary Key
        public int Id { get; set; }

        // Nome do Módulo
        [Required(ErrorMessage = "O nome do módulo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        // Descrição do Módulo
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        // Foreign Key da categoria
        [Required(ErrorMessage = "Selecione uma categoria")]
        public int CategoriaId { get; set; }

        // Propriedade de navegação: acesso á categoria pai
        // NÃO valida esta propriedade (apenas a chave estrangeira)
        public Categoria Categoria { get; set; } = null!;

        // Data de criação
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataUltimaAlteracao { get; set; }

        // Relação 1 módulo para n documentos
        public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    }
}


