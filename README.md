# IP-Monitor

A aplicação trata-se de uma interface mais intuitiva para o famoso teste de "Ping".
Basicamente o programa recebe as informações via json e então começa a "pingar" para o endereço, a diferença é que por meio do arquivo json (IPMonitor.json) pode-se controlar os parametros do teste, tamanho de bytes, tempo limite de resposta, o intervalo de um "ping" para outro.

Essa ferramenta é util para saber se o seu provedor tem uma boa latência com o servidor da sua empresa.

![IPMonitor](https://user-images.githubusercontent.com/32250409/85046334-aa4a5480-b166-11ea-9d31-02576269d61d.png)

Exemplo de como os dados devem estar no arquivo "IPMonitor.json" exatamente esse nome:
```
{
"tagServidor1" : "Google - ",
"Servidor1" : "www.google.com",
"bytes1" : 32,
"tempoResposta1": 120,
"PingMS1" : 500,
	
"tagServidor2" : "YouTube - ",
"servidor2" : "www.youtube.com",
"bytes2" : 32,
"tempoResposta2": 120,
"PingMS2" : 500,

"tagServidor3" : "Stackoverflow - ",
"servidor3" : "stackoverflow.com",
"bytes3" : 32,
"tempoResposta3": 120,
"PingMS3" : 500,
}
```
