# Model wolves playing like villagers
# pbae
# 9/2/12

## PARAMETERS
SIZE = 13
WOLVES = 3

# allowing the no-lynch-the-v-peek condiition is much to powerful
x = vector()
system.time(for (i in 1:50000) x[i] = simulate.game(T))
table(x>0)/length(x)
#table(x)/length(x)

#simulate.game(T,T,VERBOSE=T)

################################################
# add on a bit to do recusive simulations of small bits at a time

SIZE = 9
WOLVES = 2
n = 100000

# rand nk
x = replicate(n, simulate.shc.fps(0, T, F, T))
table(x %% 10)/n
x = replicate(n, simulate.shc.fps(0, F, F, T))
table(x %% 10)/n

x = replicate(n, simulate.shc.fps(0, F, F))
table(x %% 10)/n

x = replicate(n, simulate.shc.fps(-1, F, F))
table(x %% 10)/n
x = replicate(n, simulate.shc.fps(-1, T, F))
table(x %% 10)/n
x = replicate(n, simulate.shc.fps(-1, T, T))
table(x %% 10)/n

x = replicate(n, simulate.shc.fps(1, F, F))
table((x %% 10)[x < 10])/sum(x < 10)
x = replicate(n, simulate.shc.fps(1, T, F))
table((x %% 10)[x < 10])/sum(x < 10)
x = replicate(n, simulate.shc.fps(1, T, T))
table((x %% 10)[x < 10])/sum(x < 10)

#table((x %% 10)[x >= 10])/sum(x >= 10)


rm(return.value,players,peek,all.peeks,lynch, correct.peeks, peeks.living, correct.peeks.living)

# 9er SHC/FPS
simulate.shc.fps = function(fps=-1, use.shc = T, shc.d3 = F, rand.nk=F) {
	return.value=0
	players = c(1:9)
	
	# n0 peek
	peek = 8
	
	# fake peeks:
	all.peeks = c((floor(runif(8) * 8) + 1) + c(1:8), 8)
	all.peeks[all.peeks > 9] = all.peeks[all.peeks > 9] - 9
	
	# d1 lynch: vanillager
	lynch = c(3:8)[floor(runif(1) * 6) + 1]
	players = players[-lynch]
	peek = peek[peek != lynch]

	# among the vanillagers remaining (5), wolves either fps or shc
	correct.peeks = all.peeks > 2
	peeks.living = !is.na(match(all.peeks, players))
	correct.peeks.living = sum((correct.peeks & peeks.living)[-c(1,2,lynch)])
	
	####### seer hunt
	if (fps == -1 & rand.nk == F) {
		# kill a guy with a correct peek
		eligible.villagers = players[which((correct.peeks)[-c(1,2,lynch)])+2]
		
		nk = nk.na(eligible.villagers)
		
		shc = 0
		# seer hunt
		if (nk == 9) { # seer hunt successful
			return.value = return.value + 10 # n1 nk seer
		} else { # seer hunt fails
			if(use.shc) shc <- all.peeks[nk]
			peek = c(peek, peek.na(players, peek))
		}
		players = players[players != nk]
		peek = peek[peek != nk]

		# seer claims (if alive).
		lynch = postclaim.lynch(players[players != shc],peek)
		players = players[players != lynch]
		peek = peek[peek != lynch]
		
		# if seer is alive nk him otherwise
		if (max(players)==9) {
			players = players[players != 9]
		} else {
			nk = nk.na(players,peek)
			players = players[players != nk]
			peek = peek[peek != nk]
		}
		
	} else if (fps == 1 & rand.nk == F) { # use fps n1
		# if there are no fps'able wolves
		if (sum((!correct.peeks & peeks.living)[-c(1,2,lynch)]) == 0) {
			return.value = return.value + 10
			eligible.villagers = players[-c(1,2)]
		} else {
			eligible.villagers = players[which((!correct.peeks & peeks.living)[-c(1,2,lynch)])+2]
		}	
		
		nk = nk.na(eligible.villagers)
		ifelse(use.shc, shc <- all.peeks[nk], shc <- 0)
		players = players[players != nk]
		peek = c(peek, peek.na(players, peek))
		peek = peek[peek != nk]

		# seer claims (if alive). lynch based on SHC?
		lynch = postclaim.lynch(players[players != shc],peek)
		players = players[players != lynch]
		peek = peek[peek != lynch]
		
		# nk the seer
		players = players[players != 9]
	} else if (fps == 0) { # rand the NK
		nk = nk.na(players)
		
		shc = 0
		if (nk == 9) { # seer hunt successful
			return.value = return.value + 10 # n1 nk seer
			if (rand.nk == F) peek = numeric()
		} else { # seer hunt fails
			if(use.shc) shc <- all.peeks[nk]
			peek = c(peek, peek.na(players, peek))
		}
		players = players[players != nk]
		peek = peek[peek != nk]
	
		# seer claims (if alive).
		lynch = postclaim.lynch(players[players != shc],peek)
		players = players[players != lynch]
		peek = peek[peek != lynch]
		
		# if seer is alive nk him otherwise
		if (max(players)==9) {
			players = players[players != 9]
		} else {
			nk = nk.na(players,peek)
			players = players[players != nk]
			peek = peek[peek != nk]
		}
	} else if (rand.nk == F) {
		nk = nk.na(players)	
		players = players[players != nk]
		peek = peek[peek != nk]
		
		shc = 0
		if(use.shc) shc <- all.peeks[nk]
		lynch = postclaim.lynch(players[players != shc],peek)
		players = players[players != lynch]
		peek = peek[peek != lynch]
		
		# if seer is alive nk him otherwise
		if (max(players)==9) {
			players = players[players != 9]
		} else {
			nk = nk.na(players,peek)
			players = players[players != nk]
			peek = peek[peek != nk]
		}
	} else stop("fps invalid")
		
	mislynches.left = 2 - sum(players <= 2)

	while(mislynches.left >= 0) {
		if (shc.d3 & length(players)==5) {
			eligible <- players[players != shc]
		} else {
			eligible = players
		}
		lynch = postclaim.lynch(eligible,peek)		
		players = players[players != lynch]
		peek = peek[peek != lynch]
		
		if(lynch > 2) mislynches.left <- mislynches.left - 1
		if(sum(players<=2) == 0) return(return.value + 1) # village win
		
		# the NK
		nk = nk.na(players,peek)
		players = players[players != nk]
		peek = peek[peek != nk]
	}
	
	return(return.value)
	}
	
################################################

simulate.game <- function(n0.villa.peek=F,VERBOSE=F) {
	# returns 1 for village win, 2 for seer-locked win, 0 for wolf win
	
	# 'players' is a vector; 1:WOLVES are the wolves
	# the last element in the vector represents the seer
	# the rest are vanillagers
	players = c(1:SIZE)
	
	# n0 peek; "peek" is a vector of all the seer's surviving peeks
	peek = ifelse(n0.villa.peek, SIZE-1, floor(runif(1) * (SIZE-1)) + 1)
	
	if (VERBOSE) print(paste("n0 peek: ", peek))
	
	# disregard this. this is for tracking more detailed stats later
	day = 1
	
	mislynches.left = (SIZE - (WOLVES * 2 + 1)) / 2
	
	seer.alive = T
	
	while(seer.alive) {
		if (VERBOSE) print(players)
		# check for seer lock; village win
		if(check.seer.lock(players,peek)) return(2)
		
		lynch = preclaim.lynch1(players,peek)
		
		if (mislynches.left <= 1 & !lynch %in% peek[peek <= WOLVES]) {
		# seer claims unless a peeked wolf is to be lynched 
		lynch = 0
		} 
		if (VERBOSE) print(paste("lynch: ", lynch))
		
		if(lynch) { # regular lynch; lynch > 0
		if(lynch > WOLVES) mislynches.left = mislynches.left - 1
		players = players[players != lynch]
		peek = peek[peek != lynch]
		the.nk = nk.na(players)
		# day = day + 1
		if(the.nk == SIZE) seer.alive=F
		if (VERBOSE) print(paste("nk: ", the.nk))
		
		# get seer's peek
		if(seer.alive) peek = c(peek, peek.na(players,peek))
		players = players[players != the.nk]
		peek = peek[peek != the.nk]
		if (VERBOSE) print(paste("peek: ", peek))
		
		} else { # lynch = 0; seer has claimed
		lynch = postclaim.lynch(players,peek)
		if (VERBOSE) {
		print("seer claims")
		print(paste("lynch: ", lynch))
		}
		
		players = players[players != lynch]
		peek = peek[peek != lynch]
		if(lynch > WOLVES) mislynches.left = mislynches.left - 1
		
		# nk the seer
		players = players[players < SIZE]
		seer.alive = F
		# day = day + 1
		}
	}
	
	# when seer is dead
	while(mislynches.left >= 0) {
		if (VERBOSE) print(players)
		
		if(length(players) %% 2 == 0) print("even # of players")
		lynch = postclaim.lynch(players,peek)
		if (VERBOSE) print(paste("lynch: ", lynch))
		
		players = players[players != lynch]
		peek = peek[peek != lynch]
		if(lynch > WOLVES) mislynches.left = mislynches.left - 1
		if(sum(players<=WOLVES) == 0) return(1)
		
		# the NK
		the.nk = nk.na(players,peek)
		day = day + 1
		if (VERBOSE) print(paste("nk: ", the.nk))
		players = players[players != the.nk]
		peek = peek[peek != the.nk]
	}
	
	return(0)
}

peek.na <- function(players, peek) {
	# return the seer's new peek
	# pl = all players less seer less those he already peeked
	pl = players[!players %in% c(peek,SIZE)]
	
	return( pl[floor(runif(1) * length(pl) ) + 1] )
}

nk.na <- function(players,peek=0) {
	# kill randomly if seer is alive.
	# kill peeked villagers after seer dies.
	if(max(c(0,peek))>WOLVES) return(max(peek))
	pl = players[players > WOLVES]
	nk = floor(runif(1) * length(pl)) + 1
	return(pl[nk])
}

check.seer.lock = function(players, peek) {
	if(length(peek) == 0) return(0)
	if(sum(players<=WOLVES) == sum(peek<=WOLVES)) return(1)
	if(sum(peek > WOLVES) >= (length(players)-1)/2) return(1)
	return(0)
}

postclaim.lynch = function(players,peek) {
	# lynch peeked wolves, then randomly
	if (min(c(99,peek)) <= WOLVES) return(min(peek))
	
	# lynch non-peeks
	pl = players[players < SIZE & !players %in% peek] 
	return(pl[floor(runif(1) * length(pl)) + 1])
}

preclaim.lynch1 = function(players, peek) {
	# lynch at random, except that seer peeked villagers are immune
	pl = players[!players %in% peek]
	lynch = pl[floor(runif(1) * length(pl)) + 1]

	# seer claims if he is wagoned
	if (lynch == SIZE) return(0)

	# otherwise lynch the dude
	return(lynch)
}

# model how likely some is to be lynched depending on peeks
# 
#x=10000
#table(tapply(rep(3,x), seq(x), peeked.v.lynched))
#8/9

vote = function(x, n) {
	# x is array denoting player position
	# x votes for a player in {n}, but not himself
	y = floor(runif(1) * (n-1))+1
	return(ifelse(y==x, n, y))
	}
	
peeked.v.lynched = function(n, peeks=1) {
	# peeks is the number of villager peeks
	# n = players
	# each player's vote (seer's vote last):
	n=13
	c(floor(runif(n-1) * n)+1, floor(runif(1) * (n-1))+1)
	votes = table(c(floor(runif(n-1) * n)+1, floor(runif(1) * (n-1))+1))
	if (n %in% names(votes[votes==max(votes)])) {
		return(!floor(runif(1)*length(names(votes[votes==max(votes)]))))
	} else { return (0) }
}

preclaim.lynch2 = function(players, peek, claim.light=F) {
	# the more complicated model for who gets lynched
	# lynch is determined by this rule:
	# wolves and villagers are rand to vote
	# seer will not vote his v peeks; will vote wolve peeks
	n = length(players)

	# if there are no peeked wolves: peeked villager (n-2)/(n-1) times as likely to be lynched
	# if there is a peeked wolf: peeked wolves (n-1)/(n-2) times as likely to be lynched
	sum(peek > WOLVES)
	sum(!peek > WOLVES)
	
	rand = floor(runif(1) * (n*(n-1) - sum(peek > WOLVES))) + 1

	# max votes on each player
	max.votes = ifelse(players %in% peek[peek>WOLVES], n-2, n-1)

	# check if seer or peeked villa is to be lynched
	lynch = players[as.numeric(which(rand <= tapply(seq(players), seq(players), function(x) sum(max.votes[1:x])))[1])]

	# seer claims
	if (lynch == SIZE) return(0)
	if (claim.light & lynch %in% peek[peek > WOLVES]) return(0)

	# lynch the dude
	if (!lynch %in% players) print("lynch not in players")
	return(lynch) 
}

