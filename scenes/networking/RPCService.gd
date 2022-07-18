extends Node

var client: ClientRPC
var server: ServerRPC
var sync_input: SyncInput

func _ready():
    client = ClientRPC.new()
    add_child(client)

    server = ServerRPC.new()
    add_child(server)

    sync_input = SyncInput.new()
    add_child(sync_input)

    client.link_service(self)
    server.link_service(self)