using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace gsdggs
{
    internal class Program
    {
        static string[,] pacientes = new string[100, 5];
        static string[,] atendimentos = new string[100, 6];
        static int linhaAtualPaciente = 0, opcaoBuscaPaciente = 0, opcaoAtendimento = 0, opcaoPesquisaDeTempo = 0, id, linhaAtualAtendimento = 0;
        static long codigo = 1;

        static void Main(string[] args)
        {

            int opcao = 0, idade = 0;
            do
            {
                string nome = "", nomeMae = "", cpf = "", sexo = "", dataNasc = "", paramDeBusca = "";
                Console.Write("\n[1] Cadastrar Paciente \n[2] Novo Atendimento \n[3] Exibir Pacientes \n[4] Lista de Atendimentos \n[5] Código dos Procedimentos \n[6] Tempo Total dos Procedimentos \n[7] Sair\n\nEscolha uma opção: ");
                opcao = int.Parse(Console.ReadLine());

                switch (opcao)
                {
                    case 1:
                        while (nome == "")
                        {
                            Console.Write("\nInsira o nome do paciente: ");
                            nome = Console.ReadLine().ToLower();
                        }

                        Console.Write("Insira o nome da mãe do paciente (Opcional): ");
                        nomeMae = Console.ReadLine().ToLower();

                        while (dataNasc == "")
                        {
                            Console.Write("Insira a data de nascimento do paciente (dd/mm/yyyy): ");
                            dataNasc = Console.ReadLine();

                            while (!formatoValidoData(dataNasc))
                            {
                                Console.Write("Informe um formato de data válido (dd/mm/yyyy): ");
                                dataNasc = Console.ReadLine();
                            }

                            idade = calcularIdade(dataNasc);
                        }

                        while (sexo == "")
                        {
                            Console.Write("Insira o sexo do paciente (M/F): ");
                            sexo = Console.ReadLine().ToLower();

                            while (sexo.Length != 1)
                            {
                                Console.Write("Informe um formato correto para o sexo (M/F): ");
                                sexo = Console.ReadLine().ToLower();
                            }
                        }

                        while (cpf == "")
                        {
                            Console.Write("Insira o CPF do paciente (Apenas números): ");
                            cpf = Console.ReadLine();

                            while (cpf.Length != 11)
                            {
                                Console.Write("Informe um formato correto para o CPF: ");
                                cpf = Console.ReadLine();
                            }
                        }

                        if (idade > 12 && !verificarCpfRepetido(cpf))
                        {
                            cadastrarPaciente(nome, nomeMae, dataNasc, sexo, cpf);
                            if (opcao == 7) break;
                            Console.WriteLine("\nPaciente cadastrado.");
                        }
                        else if (verificarCpfRepetido(cpf))
                        {
                            if (opcao == 7) break;
                            Console.WriteLine("\nCPF já cadastro anteriormente.");
                        }
                        else
                        {
                            if (opcao == 7) break;
                            Console.WriteLine("\nO cadastro é permitido apenas para pacientes com mais de 12 anos.");
                        }
                        break;

                    case 2:
                        Console.Write("\nSelecione o paciente, para o atendimento: \n\n[1] Nome \n[2] Data de Nascimento \n[3] CPF \n\nSelecione uma opção: ");
                        opcaoBuscaPaciente = int.Parse(Console.ReadLine());

                        if (opcaoBuscaPaciente == 1)
                        {
                            Console.Write("\nInforme o nome do paciente: "); paramDeBusca = Console.ReadLine().ToLower();
                        }
                        else if (opcaoBuscaPaciente == 2)
                        {
                            Console.Write("\nInforme a data de nascimento do paciente (dd/mm/yyyy): "); paramDeBusca = Console.ReadLine();

                            while (!formatoValidoData(paramDeBusca))
                            {
                                Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                                paramDeBusca = Console.ReadLine();
                            }
                        }
                        else if (opcaoBuscaPaciente == 3)
                        {
                            Console.Write("\nInforme o CPF do paciente (Apenas números): "); paramDeBusca = Console.ReadLine();
                            while (paramDeBusca.Length != 11)
                            {
                                Console.Write("\nInforme um formato correto para o CPF: ");
                                paramDeBusca = Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nOpção inválida!");
                        }

                        bool usuarioCadastrado = procurarPaciente(paramDeBusca);

                        if (usuarioCadastrado)
                        {
                            Console.WriteLine("\nPaciente encontrado!\n");
                            Console.Write("Insira a data do atendimento (dd/mm/yyyy): "); string dataAtendimento = Console.ReadLine();

                            while (!formatoValidoData(dataAtendimento))
                            {
                                Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                                dataAtendimento = Console.ReadLine();
                            }

                            Console.Write("\nInforme qual procedimento será realizado: \n[1] Raio - X de Tórax em PA \n[2] Ultrassonografia Obstétrica \n[3] Ultrassonografia de Próstata \n[4] Tomografia \n\nSelecione uma opção: ");
                            opcaoAtendimento = int.Parse(Console.ReadLine());

                            salvarAtendimento(dataAtendimento);
                        }
                        else
                        {
                            Console.WriteLine("\nPaciente não encontrado!");
                        }
                        break;

                    case 3:
                        Console.WriteLine("\nSegue a lista de todos os pacientes cadastrados: \n");
                        for (int i = 0; i < linhaAtualPaciente; i++)
                        {
                            Console.Write(pacientes[i, 0] + "  " + pacientes[i, 2]);
                            Console.WriteLine();
                        }
                        break;

                    case 4:
                        Console.Write("\nInforme a data desejada (dd/mm/yyyy): ");
                        string dataDeBusca = Console.ReadLine();

                        while (!formatoValidoData(dataDeBusca))
                        {
                            Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                            dataDeBusca = Console.ReadLine();
                        }

                        Console.WriteLine();
                        atendimentosDataEspecifica(dataDeBusca);
                        break;

                    case 5:
                        Console.Write("\nInforme o intervalo que deseja pesquisar: \n\nData inicial: ");
                        string dataIncialPesquisa = Console.ReadLine();

                        while (!formatoValidoData(dataIncialPesquisa))
                        {
                            Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                            dataIncialPesquisa = Console.ReadLine();
                        }

                        Console.Write("\nData final: ");
                        string dataFinalPesquisa = Console.ReadLine();

                        while (!formatoValidoData(dataFinalPesquisa))
                        {
                            Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                            dataFinalPesquisa = Console.ReadLine();
                        }

                        Console.WriteLine();
                        intervaloDataParaPesquisaDeCodigo(dataIncialPesquisa, dataFinalPesquisa);
                        break;

                    case 6:
                        Console.Write("informe o procedimento desejado: \n\n[1] Raio - X de Tórax em PA \n[2] Ultrassonografia Obstétrica \n[3] Ultrassonografia de Próstata \n[4] Tomografia \n\nSelecione uma opção: ");
                        opcaoPesquisaDeTempo = int.Parse(Console.ReadLine());

                        Console.Write("\nInforme a data de inicio: ");
                        string dataInicioPesquisaTempo = Console.ReadLine();

                        while (!formatoValidoData(dataInicioPesquisaTempo))
                        {
                            Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                            dataInicioPesquisaTempo = Console.ReadLine();
                        }

                        Console.Write("Informe a data final: ");
                        string dataFinalPesquisaTempo = Console.ReadLine();

                        while (!formatoValidoData(dataFinalPesquisaTempo))
                        {
                            Console.Write("\nInforme um formato de data válido (dd/mm/yyyy): ");
                            dataFinalPesquisaTempo = Console.ReadLine();
                        }

                        Console.WriteLine();
                        intervaloDataParaPesquisaDeTempo(dataInicioPesquisaTempo, dataFinalPesquisaTempo);
                        break;

                    case 7:
                        break;

                    default:
                        Console.WriteLine("\nOpção inválida! Tente novamente.");
                        break;
                }
            } while (opcao != 7);
        }

        static void atendimentosDataEspecifica(string dataDeBusca)
        {
            DateTime dataDeBuscaConveritda = DateTime.Parse(dataDeBusca);
            bool verificarAtendimento = false;

            for (int linha = 0; linha < linhaAtualAtendimento; linha++)
            {
                DateTime dataAtendimento = DateTime.Parse(atendimentos[linha, 3]);
                for (int coluna = 0; coluna < atendimentos.GetLength(1); coluna++)
                {
                    if (dataAtendimento == dataDeBuscaConveritda)
                    {
                        Console.Write(atendimentos[linha, coluna] + "  ");
                        verificarAtendimento = true;
                    }
                }
                Console.WriteLine();
            }
            if (!verificarAtendimento) Console.WriteLine("Nenhum atendimento para essa data.");
        }

        static void intervaloDataParaPesquisaDeTempo(string dataInicioPesquisaTempo, string dataFinalPesquisaTempo)
        {
            DateTime dataInicialConvertida = DateTime.Parse(dataInicioPesquisaTempo);
            DateTime dataFinalConvertida = DateTime.Parse(dataFinalPesquisaTempo);
            int tempoTotal = 0;

            for (int linha = 0; linha < linhaAtualAtendimento; linha++)
            {
                DateTime dataAtendimentoConvertido = DateTime.Parse(atendimentos[linha, 3]);
                if (dataAtendimentoConvertido >= dataInicialConvertida && dataAtendimentoConvertido <= dataFinalConvertida)
                {
                    if (opcaoPesquisaDeTempo >= 1 && opcaoPesquisaDeTempo <= 4)
                    {
                        string[] opcoesAtendimento = { "Raio - X de Tórax em PA", "Ultrassonografia Obstétrica", "Ultrassonografia de Próstata", "Tomografia" };

                        if (atendimentos[linha, 2] == opcoesAtendimento[opcaoPesquisaDeTempo - 1])
                        {
                            tempoTotal += int.Parse(atendimentos[linha, 5]);
                        }
                    }
                }
            }
            int tempoEmHora = tempoTotal / 60;
            if (tempoTotal > 0)
            {
                if (tempoEmHora >= 1)
                {
                    Console.WriteLine($"Foram {tempoEmHora} horas de procedimento realizados no período especificado.");
                }
                else
                {
                    Console.WriteLine($"Foram {tempoTotal} minutos de procedimento realizados no período especificado.");
                }
            }
            else
            {
                Console.WriteLine("Não houve atendimentos registrados no período especificado.");
            }
        }

        static void intervaloDataParaPesquisaDeCodigo(string dataInicialPesquisa, string dataFinalPesquisa)
        {
            DateTime dataInicialConvertida = DateTime.Parse(dataInicialPesquisa);
            DateTime dataFinalConvertida = DateTime.Parse(dataFinalPesquisa);

            for (int linha = 0; linha < linhaAtualAtendimento; linha++)
            {
                DateTime dataAtendimentoConvertido = DateTime.Parse(atendimentos[linha, 3]);
                if (dataAtendimentoConvertido >= dataInicialConvertida && dataAtendimentoConvertido <= dataFinalConvertida)
                {
                    Console.Write(atendimentos[linha, 4] + " " + atendimentos[linha, 2]);
                }
                Console.WriteLine();
            }
        }

        static void salvarAtendimento(string dataAtendimento)
        {
            if (opcaoAtendimento == 1)
            {
                atendimentos[linhaAtualAtendimento, 0] = pacientes[id, 0];
                atendimentos[linhaAtualAtendimento, 1] = pacientes[id, 4];
                atendimentos[linhaAtualAtendimento, 2] = "Raio - X de Tórax em PA";
                atendimentos[linhaAtualAtendimento, 3] = dataAtendimento;
                atendimentos[linhaAtualAtendimento, 4] = codigo.ToString("D10");
                atendimentos[linhaAtualAtendimento, 5] = "10";
                Console.WriteLine("\nProcedimento agendado!");
                codigo++;
                linhaAtualAtendimento++;
            }
            else if (opcaoAtendimento == 2)
            {
                if (pacientes[id, 3] == "f" && calcularIdade(pacientes[id, 2]) < 60)
                {
                    atendimentos[linhaAtualAtendimento, 0] = pacientes[id, 0];
                    atendimentos[linhaAtualAtendimento, 1] = pacientes[id, 4];
                    atendimentos[linhaAtualAtendimento, 2] = "Ultrassonografia Obstétrica";
                    atendimentos[linhaAtualAtendimento, 3] = dataAtendimento;
                    atendimentos[linhaAtualAtendimento, 4] = codigo.ToString("D10");
                    atendimentos[linhaAtualAtendimento, 5] = "35";
                    Console.WriteLine("\nProcedimento agendado!");
                    codigo++;
                    linhaAtualAtendimento++;
                }
                else
                {
                    Console.WriteLine("\nProcedimento realizado só por Pacientes do sexo Feminino com idade menor que 60 anos.");
                }
            }
            else if (opcaoAtendimento == 3)
            {
                if (pacientes[id, 3] == "m")
                {
                    atendimentos[linhaAtualAtendimento, 0] = pacientes[id, 0];
                    atendimentos[linhaAtualAtendimento, 1] = pacientes[id, 4];
                    atendimentos[linhaAtualAtendimento, 2] = "Ultrassonografia de Próstata";
                    atendimentos[linhaAtualAtendimento, 3] = dataAtendimento;
                    atendimentos[linhaAtualAtendimento, 4] = codigo.ToString("D10");
                    atendimentos[linhaAtualAtendimento, 5] = "20";
                    Console.WriteLine("\nProcedimento agendado!");
                    codigo++;
                    linhaAtualAtendimento++;
                }
                else
                {
                    Console.WriteLine("\nProcedimento realizado somente por Pacientes do sexo Masculino.");
                }
            }
            else if (opcaoAtendimento == 4)
            {
                int cpf = 0;
                for (int linha = 0; linha < linhaAtualAtendimento; linha++)
                {
                    if (atendimentos[linha, 1] == pacientes[id, 4])
                    {
                        if (atendimentos[linha, 2] == "Ultrassonografia Obstétrica" || atendimentos[linha, 2] == "Ultrassonografia de Próstata")
                        {
                            cpf = linha;
                        }
                    }
                }

                if (atendimentos[cpf, 2] == "Ultrassonografia Obstétrica" || atendimentos[cpf, 2] == "Ultrassonografia de Próstata")
                {
                    if (verificarPrazoTomografia(cpf, dataAtendimento))
                    {
                        atendimentos[linhaAtualAtendimento, 0] = pacientes[id, 0];
                        atendimentos[linhaAtualAtendimento, 1] = pacientes[id, 4];
                        atendimentos[linhaAtualAtendimento, 2] = "Tomografia";
                        atendimentos[linhaAtualAtendimento, 3] = dataAtendimento;
                        atendimentos[linhaAtualAtendimento, 4] = codigo.ToString("D10");
                        atendimentos[linhaAtualAtendimento, 5] = "20";
                        Console.WriteLine("\nProcedimento agendado!");
                        codigo++;
                        linhaAtualAtendimento++;
                    }
                    else
                    {
                        Console.WriteLine("\nSó pode ser realizado em Pacientes que não realizaram Ultrassonografia nos últimos três meses.");
                    }
                }
                else if (atendimentos[cpf, 2] != "Tomografia")
                {
                    atendimentos[linhaAtualAtendimento, 0] = pacientes[id, 0];
                    atendimentos[linhaAtualAtendimento, 1] = pacientes[id, 4];
                    atendimentos[linhaAtualAtendimento, 2] = "Tomografia";
                    atendimentos[linhaAtualAtendimento, 3] = dataAtendimento;
                    atendimentos[linhaAtualAtendimento, 4] = codigo.ToString("D10");
                    atendimentos[linhaAtualAtendimento, 5] = "20";
                    Console.WriteLine("\nProcedimento agendado!");
                    codigo++;
                    linhaAtualAtendimento++;
                }
                else
                {
                    atendimentos[linhaAtualAtendimento, 0] = pacientes[id, 0];
                    atendimentos[linhaAtualAtendimento, 1] = pacientes[id, 4];
                    atendimentos[linhaAtualAtendimento, 2] = "Tomografia";
                    atendimentos[linhaAtualAtendimento, 3] = dataAtendimento;
                    atendimentos[linhaAtualAtendimento, 4] = codigo.ToString("D10");
                    atendimentos[linhaAtualAtendimento, 5] = "20";
                    Console.WriteLine("\nProcedimento agendado!");
                    codigo++;
                    linhaAtualAtendimento++;
                }
            }
            else
            {
                Console.WriteLine("Opção Inválida!");
            }
        }



        static bool verificarPrazoTomografia(int linha, string dataAtendimento)
        {
            DateTime atendimentoAterior = DateTime.Parse(atendimentos[linha, 3]);
            DateTime dataAtendimentoConertido = DateTime.Parse(dataAtendimento);

            TimeSpan diferencaTotal = dataAtendimentoConertido - atendimentoAterior;
            int diferencaMeses = (int)(diferencaTotal.TotalDays / 30.44);

            return diferencaMeses >= 3;
        }

        static bool procurarPaciente(string paramBusca)
        {
            if (opcaoBuscaPaciente == 1)
            {
                for (int linha = 0; linha < pacientes.GetLength(0); linha++)
                {
                    if (pacientes[linha, 0] == paramBusca) { id = linha; return true; }
                }
            }
            else if (opcaoBuscaPaciente == 2)
            {
                for (int linha = 0; linha < pacientes.GetLength(0); linha++)
                {
                    if (pacientes[linha, 2] == paramBusca) { id = linha; return true; }
                }
            }
            else if (opcaoBuscaPaciente == 3)
            {
                for (int linha = 0; linha < pacientes.GetLength(0); linha++)
                {
                    if (pacientes[linha, 4] == paramBusca) { id = linha; return true; }
                }
            }
            id = int.MaxValue;
            return false;
        }

        static bool formatoValidoData(string dataNasc)
        {
            Regex formatoValido = new Regex(@"\d{2}/\d{2}/\d{4}$");

            if (formatoValido.IsMatch(dataNasc) == true) { return true; }
            return false;
        }

        static bool verificarCpfRepetido(string cpf)
        {
            for (int linha = 0; linha < pacientes.GetLength(0); linha++)
            {
                if (pacientes[linha, 4] == cpf) return true;
            }
            return false;
        }

        static int calcularIdade(string dataNasc)
        {
            DateTime dataNascCovertido = DateTime.Parse(dataNasc);

            TimeSpan diferença = DateTime.Now - dataNascCovertido;

            int idade = (int)Math.Floor(diferença.TotalDays / 365.25);

            return idade;
        }

        static void cadastrarPaciente(string nome, string nomeMae, string dataNasc, string sexo, string cpf)
        {
            for (int coluna = 0; coluna < pacientes.GetLength(1); coluna++)
            {
                pacientes[linhaAtualPaciente, 0] = nome;
                pacientes[linhaAtualPaciente, 1] = nomeMae;
                pacientes[linhaAtualPaciente, 2] = dataNasc;
                pacientes[linhaAtualPaciente, 3] = sexo;
                pacientes[linhaAtualPaciente, 4] = cpf;
            }
            linhaAtualPaciente++;
        }
    }
}