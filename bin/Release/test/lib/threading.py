

# Thread testing ======================================
def do_process():
	prog = 0
	sim.status("processing.")
	sim.busy(True)
	while prog < 100:
		sim.sleep(100)
		sim.status("%s done." % str(prog)+"%")
		prog += 1
		sim.progress(prog)
	sim.status("Done.")
	sim.busy(False)

def deploy():
	import thread
	thread.start_new(do_process,())
# =====================================================