$Haste::Ranks[0] = "Newbie	0	00ff00";
$Haste::Ranks[1] = "Plebian	3	40fa00";
$Haste::Ranks[2] = "Ameteur	7	7ff500";
$Haste::Ranks[3] = "Casual	12	baf000";
$Haste::Ranks[4] = "Novice	18	ebe200";
$Haste::Ranks[5] = "Avid	25	e6a200";
$Haste::Ranks[6] = "Decent	33	e16400";
$Haste::Ranks[7] = "Sharp	42	dc2800";
$Haste::Ranks[8] = "Pro	52	d70010";
$Haste::Ranks[9] = "Athlete 63	d20046";
$Haste::Ranks[10] = "Beast	75	cd007a";
$Haste::Ranks[11] = "Unstoppable	88	c800ab";
$Haste::Ranks[12] = "Godlike	102	ad00c3";
$Haste::Ranks[13] = "Immortal	117	7700bf";
$Haste::Ranks[14] = "Universal	133	5500c3";

function GameConnection::getRankName(%this,%id) {
	if(%id >= 15) {
		return "<color:00ffff>Plus" SPC mCeil((%this.score - 150) / 25);
	}
	return "<color:" @ getField($Haste::Ranks[%id], 2) @ ">" @ getField($Haste::Ranks[%id], 0);
}

function GameConnection::getRank(%this) {
	if(%this.score < 150) {
		for(%i=0;%i<15;%i++) {
			%data = $Haste::Ranks[%i];
			%amnt[min] = getField(%data, 1);

			if($Haste::Ranks[%i+1] !$= "") {
				%data = $Haste::Ranks[%i+1];
				%amnt[max] = getField(%data, 1);
			} else {
				%amnt[max] = 150;
			}
			%data = $Haste::Ranks[%i];

			if(%this.score >= %amnt[min] && %this.score < %amnt[max]) {
				echo("tried" SPC %i SPC %data SPC "[" @ %amnt[min] SPC %amnt[max] @ "]");
				break;
			}
		}

		return %i;
	}
	return 15;
}
function GameConnection::setRank(%this) {
	%this.rank = %this.getRank();
	%this.rankName = %this.getRankName(%this.rank);

	%this.clanPrefix = "\c6[" @ %this.rankName @ "\c6]\c7" SPC %this.origPrefix;
}