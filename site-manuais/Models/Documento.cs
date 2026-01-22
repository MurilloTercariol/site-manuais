using System.ComponentModel.DataAnnotations;

namespace site_manuais.Models;

public class Documento
{
    // Primary Key
    public int Id { get; set; }

    // Título do documento (ex: "Como cadastrar um usuário")
    [Required(ErrorMessage = "O título do documento é obrigatório")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    // Descrição
    [StringLength (1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    // Caminho onde o PDF está salvo
    // Ex: "/uploads/docs/cadastro-usuario.pdf"
    public string CaminhoArquivo { get; set; } = string.Empty;

    // Nome original do arquivo
    public string NomeArquivoOriginal { get; set; } = string.Empty;

    // Tamanho do arquivo em bytes
    public long TamanhoArquivo { get; set; }

    // Foreign Key módulo
    [Required(ErrorMessage = "Selecione o módulo ao qual o documento pertence")]
    public int ModuloId { get; set; }

    // Propriedade de navegação: acesso ao módulo pai
    public Modulo Modulo { get; set; } = null!;

    // Data de upload
    public DateTime DataUpload { get; set; } = DateTime.Now;

    // Data da última atualização
    public DateTime? DataUltimaAlteracao { get; set; }

}
