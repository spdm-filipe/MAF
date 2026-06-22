-- Schema de exemplo com problemas propositais para o agente revisar

CREATE TABLE pedidos (
    id INT PRIMARY KEY,
    cliente_nome VARCHAR(200),
    cliente_email VARCHAR(200),
    cliente_telefone VARCHAR(20),
    produto_nome VARCHAR(200),
    produto_preco DECIMAL(10,2),
    produto_categoria VARCHAR(100),
    quantidade INT,
    data_pedido DATETIME,
    status VARCHAR(50),
    observacoes TEXT
);

CREATE TABLE logs (
    id INT PRIMARY KEY,
    mensagem TEXT,
    data DATETIME
);