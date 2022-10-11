
$ctx = New-AzStorageContext -ConnectionString "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"

$containersNames = @(
    "azure-webjobs-hosts",
    "azure-webjobs-secrets",
    "testhubname-applease",
    "testhubname-leases")

$queuesNames = @(
    "testhubname-control-00",
    "testhubname-control-01",
    "testhubname-control-02",
    "testhubname-control-03",
    "testhubname-workitems")

$tablesNames = @(
    "TestHubNameHistory",
    "TestHubNameInstances"
);

# Remove containers
foreach ( $containerName in $containersNames )
{
    Remove-AzStorageContainer -Name $containerName -Context $ctx -Force
}

# Remove queues
foreach ( $queueName in $queuesNames )
{
    Remove-AzStorageQueue -Name $queueName -Context $ctx -Force
}

# Remove tables
foreach ( $tableName in $tablesNames )
{
    Remove-AzStorageTable -Name $tableName -Context $ctx -Force
}
