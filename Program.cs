using System;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        ArvoreRubroNegra arvore = new ArvoreRubroNegra();

        Console.WriteLine("=== ÁRVORE RUBRO-NEGRA ===");
        Console.WriteLine("Digite números inteiros:");

        while (true)
        {
            string entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada)) { continue; } ;

            // Quebra a entrada em partes usando espaço ou vírgula
            string[] numeros = entrada.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in numeros)
            {
                if (int.TryParse(item, out int valor))
                {
                    arvore.Inserir(valor);
                }
                else
                {
                    Console.WriteLine($"'{item}' não é um número inteiro válido.");
                }
            }
        }
    }
}
public enum Cor
{
    Vermelho,
    Preto
}

// 2. Estrutura do Nó da Árvore
public class No
{
    public int Valor;
    public Cor Cor;
    public No Esquerdo, Direito, Pai;

    public No(int valor)
    {
        Valor = valor;
        Cor = Cor.Vermelho; // Regra: Todo novo nó nasce VERMELHO
    }
}
public class ArvoreRubroNegra
{
    private No raiz;

    // --- ROTAÇÕES (Ajustes de estrutura) ---
    private void RotacaoEsquerda(No x)
    {
        No y = x.Direito;
        x.Direito = y.Esquerdo;
        if (y.Esquerdo != null) y.Esquerdo.Pai = x;

        y.Pai = x.Pai;
        if (x.Pai == null) raiz = y;
        else if (x == x.Pai.Esquerdo) x.Pai.Esquerdo = y;
        else x.Pai.Direito = y;

        y.Esquerdo = x;
        x.Pai = y;
    }

    private void RotacaoDireita(No x)
    {
        No y = x.Esquerdo;
        x.Esquerdo = y.Direito;
        if (y.Direito != null) y.Direito.Pai = x;

        y.Pai = x.Pai;
        if (x.Pai == null) raiz = y;
        else if (x == x.Pai.Direito) x.Pai.Direito = y;
        else x.Pai.Esquerdo = y;

        y.Direito = x;
        x.Pai = y;
    }

    // --- INSERÇÃO ---
    public void Inserir(int valor)
    {
        Console.WriteLine($"\nInserindo: {valor}");
        No novo = new No(valor);

        // Caso base: Árvore vazia
        if (raiz == null)
        {
            raiz = novo;
            raiz.Cor = Cor.Preto; // Regra: A raiz DEVE ser PRETA
            ImprimirVisual();
            Console.WriteLine("Raiz definida como preta.\n");
            return;
        }

        // Passagem 1: Inserção padrão de Árvore Binária de Busca (BST)
        No pai = null;
        No atual = raiz;
        while (atual != null)
        {
            pai = atual;
            if (novo.Valor < atual.Valor) atual = atual.Esquerdo;
            else if (novo.Valor > atual.Valor) atual = atual.Direito;
            else { Console.WriteLine("número repetido encontrado e não inserido"); return; } // Evita inserir números repetidos
        }

        novo.Pai = pai;
        if (novo.Valor < pai.Valor) pai.Esquerdo = novo;
        else pai.Direito = novo;

        // Se o pai for PRETO, não há conflito de cores
        if (pai.Cor == Cor.Preto)
        {
            ImprimirVisual();
            Console.WriteLine("Sem balanceamento.\n");
            return;
        }

        // Passagem 2: Corrigir violações de cores (Dois vermelhos seguidos)
        Balancear(novo);
        ImprimirVisual();
        Console.WriteLine();
    }

    // --- BALANCEAMENTO (Correção de Regras) ---
    private void Balancear(No no)
    {
        while (no.Pai != null && no.Pai.Cor == Cor.Vermelho)
        {
            No pai = no.Pai;
            No avo = pai.Pai;

            // PAI É FILHO ESQUERDO DO AVÔ
            if (pai == avo.Esquerdo)
            {
                No tio = avo.Direito;

                // CASO 1: O Tio é VERMELHO (Muda apenas as cores)
                if (tio != null && tio.Cor == Cor.Vermelho)
                {
                    Console.WriteLine($"Caso 1: Tio ({tio.Valor}) é vermelho. Invertendo cores.");
                    pai.Cor = Cor.Preto;
                    tio.Cor = Cor.Preto;
                    avo.Cor = Cor.Vermelho;
                    no = avo; // Sobe para verificar o avô
                }
                else
                {
                    // CASO 2: O Tio é PRETO e o nó é filho DIREITO (Rotação Dupla)
                    if (no == pai.Direito)
                    {
                        Console.WriteLine($"Caso 2 (LR): Ajustando cores e rotacionando {pai.Valor} a esquerda.");
                        no = pai;
                        RotacaoEsquerda(no);
                    }
                    // CASO 3: O Tio é PRETO e o nó é filho ESQUERDO (Rotação Simples)
                    Console.WriteLine($"Caso 3 (LL): Ajustando cores e rotacionando {avo.Valor} a direita.");
                    no.Pai.Cor = Cor.Preto;
                    avo.Cor = Cor.Vermelho;
                    RotacaoDireita(avo);
                }
            }
            // PAI É FILHO DIREITO DO AVÔ (Simétrico)
            else
            {
                No tio = avo.Esquerdo;

                // CASO 1: O Tio é VERMELHO
                if (tio != null && tio.Cor == Cor.Vermelho)
                {
                    Console.WriteLine($"Caso 1: Tio ({tio.Valor}) é vermelho. Invertendo cores.");
                    pai.Cor = Cor.Preto;
                    tio.Cor = Cor.Preto;
                    avo.Cor = Cor.Vermelho;
                    no = avo;
                }
                else
                {
                    // CASO 2: O Tio é PRETO e o nó é filho ESQUERDO
                    if (no == pai.Esquerdo)
                    {
                        no = pai;
                        RotacaoDireita(no);
                    }
                    // CASO 3: O Tio é PRETO e o nó é filho DIREITO
                    Console.WriteLine($"Caso 3 (RR): {pai.Valor} fica preto e {avo.Valor} vermelho.");
                    no.Pai.Cor = Cor.Preto;
                    avo.Cor = Cor.Vermelho;
                    Console.WriteLine($"Rotacionando {avo.Valor} a esquerda.");
                    RotacaoEsquerda(avo);
                }
            }
        }

        raiz.Cor = Cor.Preto; // Garante que a raiz permaneça preta
    }

    // --- DESENHO VISUAL COM BARRA DIAGONAL ---
    public void ImprimirVisual()
    {
        if (raiz == null) return;

        // Fila para percorrer a árvore nível por nível (BFS)
        Queue<(No no, int nivel, int x)> fila = new Queue<(No, int, int)>();
        fila.Enqueue((raiz, 0, 20)); // Começa a raiz no centro da tela (coluna 20)

        int nivelAtual = 0;
        string linhaNos = "";
        string linhaTracos = "";

        Console.WriteLine();
        while (fila.Count > 0)
        {
            var (no, nivel, x) = fila.Dequeue();

            // Quando muda de nível, imprime as linhas salvas na tela
            if (nivel > nivelAtual)
            {
                Console.WriteLine(linhaNos);
                if (!string.IsNullOrWhiteSpace(linhaTracos)) Console.WriteLine(linhaTracos);

                linhaNos = "";
                linhaTracos = "";
                nivelAtual = nivel;
            }

            // Formata o texto do nó (ex: "10(P)")
            string corStr = no.Cor == Cor.Preto ? "P" : "V";
            string texto = $"{no.Valor}({corStr})";

            // Alinha o texto na coluna X correspondente
            linhaNos = linhaNos.PadRight(x) + texto;

            // Prepara as conexões para os filhos
            int deslocamento = Math.Max(2, 8 / (nivel + 1));

            if (no.Esquerdo != null)
            {
                int xEsq = x - deslocamento;
                fila.Enqueue((no.Esquerdo, nivel + 1, xEsq));
                linhaTracos = linhaTracos.PadRight(xEsq + 1) + "/";
            }
            if (no.Direito != null)
            {
                int xDir = x + deslocamento;
                fila.Enqueue((no.Direito, nivel + 1, xDir));
                linhaTracos = linhaTracos.PadRight(xDir - 1) + "\\";
            }
        }

        // Imprime a última linha restante
        Console.WriteLine(linhaNos);
        if (!string.IsNullOrWhiteSpace(linhaTracos)) Console.WriteLine(linhaTracos);
    }
}

