using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NautaRobbot;

using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using NautaRobbot.Helpers;

public class CompFileUpload : CompBase
{
    public string PlaceHolder { get; set; } = "Digite um valor aqui...";
    public string Valor { get; set; } = "";
    public string CssClass { get; set; } = "";
    public string SavePathImage { get; set; } = "";
    public string CampoSqlListagem { get; set; } = "";


    FileUpload RetornaComponente(CompFileUpload componente)
    {
        FileUpload fileUpload = new FileUpload();
        fileUpload.ID = componente.SQL.campoSQL;
        fileUpload.ClientIDMode = ClientIDMode.Static;
        fileUpload.Attributes.Add("Name", componente.SQL.campoSQL);
        fileUpload.CssClass = fileUpload.CssClass;
        fileUpload.Attributes.Add("accept", "image/*");
        fileUpload.Attributes.Add("data-preview", componente.SQL.campoSQL + "_preview");
        fileUpload.Attributes.Add("data-preview-container", "container_" + componente.SQL.campoSQL);
        return fileUpload;
    }


    public Panel MontarComponente(CompFileUpload componente,
        bool exibirTagObrigatoria = false,
        bool moduloExibicao = false,
        bool exibirUpload = false)
    {
        var panel = new Panel();
        if (moduloExibicao)
            panel.Controls.Add(MontarComponenteExibicao(componente));
        else
        {
            componente.CssClass = !componente.CssClass.Equals("") ?
               "campo-padrao " + componente.CssClass : "campo-padrao";

            panel.Attributes.Add("class", "separator_" + componente.SQL.campoSQL + " mb-3");

            //Label
            panel.Controls.Add(new LiteralControl(@"<div class=""titulo-padrao"">"
            + componente.HTML.label +
            (componente.Config.campoObrigatorio && exibirTagObrigatoria ? "*" : "")
            +
            "</div>"));

            if (!componente.Valor.Equals(""))
            {
                panel.Controls.Add(new LiteralControl(@"<div><i>Imagem Carregada:</i></div>"));
                panel.Controls.Add(new LiteralControl(@"<img id='" + componente.SQL.campoSQL + @"_img' src=""" + componente.Valor + @""" class=""img-exibicao-file-upload"" />"));
            }

            panel.Controls.Add(new LiteralControl($@"
                <div class='img-preview-container' id='container_{componente.SQL.campoSQL}' style='display: none;'>
                    <i>Preview:</i>
                    <img id='{componente.SQL.campoSQL}_preview'
                         class='img-exibicao-file-upload' />
                </div>
            "));


            //Componente
            if (exibirUpload)
                panel.Controls.Add(RetornaComponente(componente));
        }



        return panel;
    }

    public Panel MontarComponenteExibicao(CompFileUpload componente)
    {
        var panel = new Panel();
        componente.CssClass = !componente.CssClass.Equals("") ?
           "campo-padrao " + componente.CssClass : "campo-padrao";

        panel.Attributes.Add("class", "separator_" + componente.SQL.campoSQL + " mb-3");

        //Label
        panel.Controls.Add(new LiteralControl(@"<b><span id=""campo_exibir_title_" + componente.SQL.campoSQL + @""" class=""exibir_padrao_title"">"));
        panel.Controls.Add(new LiteralControl(componente.HTML.label));
        panel.Controls.Add(new LiteralControl(@":</span></b>"));
        if (!componente.Valor.Equals(""))
        {
            panel.Controls.Add(new LiteralControl(@"<div><i>Imagem Carregada:</i></div>"));
            panel.Controls.Add(new LiteralControl(@"<img id='" + componente.SQL.campoSQL + @"_img' src=""" + componente.Valor + @""" class=""img-exibicao-file-upload"" />"));
        }
        panel.Controls.Add(new LiteralControl(@"<span id=""campo_exibir_value_" + componente.SQL.campoSQL + @""" class=""exibir_padrao_value"">"));
        panel.Controls.Add(new LiteralControl(@"<img id='" + componente.SQL.campoSQL + @"_img' src=""" + componente.Valor + @""" class=""img-exibicao-file-upload"" />"));
        panel.Controls.Add(new LiteralControl(@"</span>"));
        return panel;
    }

    public void salvarArquivo(FileUpload fileUploadControl, string savePath, string fileName_alternativo = "")
    {
        if (fileUploadControl != null && fileUploadControl.HasFile)
        {
            try
            {
                // Verifica se a pasta existe, se não, cria
                string fullPath = HttpContext.Current.Server.MapPath(savePath);
                if (!System.IO.Directory.Exists(fullPath))
                {
                    System.IO.Directory.CreateDirectory(fullPath);
                }

                string fileName = fileUploadControl.FileName;
                fileName = !fileName_alternativo.Equals("") ? fileName_alternativo : fileName;

                // Define o caminho completo para salvar o arquivo
                string filePath = System.IO.Path.Combine(fullPath, fileName);

                // Salva o arquivo
                fileUploadControl.SaveAs(filePath);

                // Log de sucesso
                //HttpContext.Current.Response.Write("Upload feito com sucesso! Arquivo salvo em: " + filePath);
            }
            catch (Exception ex)
            {
                // Log de erro
                //HttpContext.Current.Response.Write("Erro ao salvar o arquivo: " + ex.Message);
            }
        }
        else
        {
            // Log quando nenhum arquivo foi selecionado
            //HttpContext.Current.Response.Write("Por favor, selecione um arquivo para upload.");
        }

        // Finaliza a resposta
        //HttpContext.Current.Response.End();
    }

}

