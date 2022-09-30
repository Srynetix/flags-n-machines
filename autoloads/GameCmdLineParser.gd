extends SxCmdLineParser

func _handle_args(args: Args) -> void:
    if args.options.has("server"):
        print("OK, SERVER MODE")