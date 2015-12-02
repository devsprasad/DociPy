

def BER(original,recieved):
	if isinstance(original,str) : original = [int(bit) for bit in list(original)]
	if isinstance(recieved,str) : recieved = [int(bit) for bit in list(recieved)]
	total_bits = len(original)
	recieved = pad(recieved,len(original))
	error_sum = 0.0
	for i in range(len(original)): 
		if original[i] != recieved[i] : error_sum += 1.0
	return error_sum/total_bits