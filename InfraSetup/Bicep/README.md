# How to setup you Aks


1 Installare AZ Cli (https://docs.microsoft.com/it-it/cli/azure/install-azure-cli)

2 Installare Bicep Extension in Visual Studio Code (https://docs.microsoft.com/it-it/azure/azure-resource-manager/bicep/install)

3 Lanciare il comando seguente per installare Azure AKS Cli per interagire con il cluster K8s

```
az aks install-cli
```

4 Lanciare comando az login e controllare la subscription abilitata

5 Lanciare il comando seguente per verificare e validare i file bicep

```
az deployment sub validate --template-file ./aks-deployment.bicep --location northeurope
```

6 Lanciare il comando per visualizzare una preview di cosa viene creato lanciando lo script bicep

```
az deployment sub create --template-file ./aks-deployment.bicep --location northeurope --what-if
```

7 Se la preview ottenuta dal comando precendete è corretta allora lanciare il seguente comando per creare realmente le risorse su Azure

```
az deployment sub create --template-file ./aks-deployment.bicep --location northeurope
```

8 Al termine dello step precedente lanciare il comando seguente per connettere la vostra macchina locale con il cluster K8s creato

```
az aks get-credentials --resource-group myResourceGroup --name myAKSCluster
```

9 Eseguire i seguenti comandi per installare il RabbitMQ nel vostro cluster k8s:

a)
```
kubectl apply -f https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml
```

b)
```
kubectl apply -f azure-vote.yaml
```


10 Lanciare il comando seguente per visualizzare "l'External-IP" associato al vostro RabbitMQ da utilizzare nelle vostre connection strings

```
kubectl get services
```

11 Una volta recuperato l'external-IP potete accedere al control panel di RabbitMQ direttamente da Browser (https://<your-external-ip>:15672 - username: guest | password: guest) 