#include <string.h>
#include "convfmml.h"
#include "modify.h"


void modlastrhythm(void)
{
  rhythm_t *rhythm;

  for (rhythm=rhythm_head; rhythm->next!=NULL; rhythm=rhythm->next)
    ;
  rhythm->endpos = totalpos;

  return;
}


track_t *func_modify_track(track_t *head)
{
  track_t *track=head;int i=0;

  while (track != NULL) {
    track = modify_track_data(track, &head);
  }

  return head;
}


track_t *modify_track_data(track_t *track, track_t **head)
{
  int i;
  int restonlyflg=1;
  note_t *tmp, *tmp2;
  pos_t pos;

  if (config.track.print_track == 2 && !istrack(track->num)) {   // delete not printing track
    return deltrackcell(track, head);
  }
  else {

    for (tmp=track->note_head; tmp!=NULL; tmp=tmp->next) {
      if (tmp->velocity > 0) {
	restonlyflg = 0;
      }
      tmp2 = tmp;
    }

    if (config.track.print_track == 1 && restonlyflg == 1) {   // delete only rest track
      return deltrackcell(track, head);
    }
    else {   // modify last rest
      if (cmppos(track->eotpos, totalpos) < 0) {
	if (tmp2->velocity == 0) {   // if last is rest
	  tmp2->endpos = totalpos;
	}
	else {   // if last is key-note
	  tmp2 = make_note_list(tmp2, tmp2->endpos, &track->note_head);
	  tmp2->endpos = totalpos;
	  tmp2->velocity = 0;
	}
      }
      config.track.tracksize++;
    }

  }

  if (ispart(config.track.tracksize) < 0) {
    fprintf(stderr, "[ERROR] 出力するトラック数がパート数の上限を超えています。\n");
    free_track_list(track);
    free_metalists();
    free_config();
    ERREXIT();
  }

  return track->next;
}


void func_modify_ccpc(track_t *track)
{
  int i=1;

  while (track != NULL) {
    if (track->note_head != NULL) {
      track->ccpc_head = modify_ccpc(track->note_head, track->ccpc_head, i++);
      modify_note_and_rest_by_ccpc(track);
    }
    track = track->next;
  }

  return;
}


ccpc_t *modify_ccpc(note_t *nhead, ccpc_t *chead, int num)
{
  note_t *nmk;
  ccpc_t *cmk;
  ccpc_t *prevl, *prepn, *prepr;

  
  prevl = NULL;
  prepn = NULL;
  prepr = NULL;

  nmk = nhead;
  cmk = chead;

  
  while (cmk != NULL) {

    if (cmppos(nmk->endpos, cmk->pos) <= 0) {   // set next note
      if (nmk->next == NULL) {   // for there is ccpc in eot
	break;
      }
      else {
	nmk = nmk->next;
	continue;
      }
    }

    cmk->modvalue = convccpcvalue(cmk, num);

    /* delete pre-data because its in rest */
    switch (cmk->type) {
    case VOLUME:
      if (config.ccpc.invalid_ccpc == 1) {
	prevl = isccpcinrest(prevl, cmk, nmk, &chead);
      }
      break;
    case PAN:
      if (ispart(num) == 0) {   // delete pan in SSG part
	cmk = delccpc(cmk, &chead);
	continue;
      }
      else if (config.ccpc.invalid_ccpc == 1) {
	prepn = isccpcinrest(prepn, cmk, nmk, &chead);
      }
      break;
    case PC:
      if (config.ccpc.invalid_ccpc == 1) {
	prepr = isccpcinrest(prepr, cmk, nmk, &chead);
      }
      break;
    }

    cmk = cmk->next;

  }   // while (cmk)
  
  /* delete pre-data because its in rest and no effect until eot */
  for ( ; nmk->next!=NULL; nmk=nmk->next)
    ;
  if (config.ccpc.invalid_ccpc == 1 && nmk->velocity == 0) {
    if (prevl != NULL && cmppos(prevl->pos, nmk->pos) >= 0) {
      delccpc(prevl, &chead);
    }
    if (prepn != NULL && cmppos(prepn->pos, nmk->pos) >= 0) {
      delccpc(prepn, &chead);
    }
    if (prepr != NULL && cmppos(prepr->pos, nmk->pos) >= 0) {
      delccpc(prepr, &chead);
    }
  }


  prevl = NULL;
  prepn = NULL;
  prepr = NULL;

  nmk = nhead;
  cmk = chead;
  

  while (cmk != NULL) {

    if (cmppos(nmk->endpos, cmk->pos) <= 0) {   // set next note
      if (nmk->next == NULL) {   // for there is ccpc in eot
	break;
      }
      else {
	nmk = nmk->next;
	continue;
      }
    }

    switch (cmk->type) {
    case VOLUME:
      cmk = ismodccpc(&prevl, cmk, nmk, &chead);
      break;
    case PAN:
      cmk = ismodccpc(&prepn, cmk, nmk, &chead);
      break;
    case PC:
      cmk = ismodccpc(&prepr, cmk, nmk, &chead);
      break;
    }

    cmk = cmk->next;

  }   // while (cmk)
  
  
  return chead;
}


ccpc_t *isccpcinrest(ccpc_t *predata, ccpc_t *nowdata, note_t *note, ccpc_t **head)
{
  note_t *tmp;

  if (predata != NULL) {
    if (note->velocity == 0 &&
	cmppos(predata->pos, note->pos) >= 0 &&
	cmppos(predata->pos, nowdata->pos) < 0) {
      delsomeccpcinrest(predata, head);
    }
    else if (note->velocity > 0 &&
	     !cmppos(nowdata->pos, note->pos) &&
	     cmppos(nowdata->pos, predata->pos) > 0) {
      for (tmp=note->prev; tmp!=NULL; tmp=tmp->prev) {
	if (tmp->velocity > 0) {
	  break;
	}
	if (cmppos(tmp->pos, predata->pos) <= 0) {
	  delsomeccpcinrest(predata, head);
	}
	if (cmppos(tmp->endpos, predata->pos) < 0) {
	  break;
	}
      }
    }
  }

  return nowdata;
}


void delsomeccpcinrest(ccpc_t *p, ccpc_t **head)
{
  ccpc_t *tmp1, *tmp2;
  
  tmp1 = p;
  while (1) {
    tmp2 = searchpre2ccpc(tmp1);
    if (tmp2 != NULL && !cmppos(tmp1->pos, tmp2->pos)) {
      delccpc(tmp1, head);
      tmp1 = tmp2;
    }
    else {
      break;
    }
  }
  delccpc(tmp1, head);
  
  return;
}


ccpc_t *searchpre2ccpc(ccpc_t *p)
{
  ccpc_t *mk;

  for (mk=p->prev; mk!=NULL; mk=mk->prev) {
    if (mk->type == p->type) {
      return mk;
    }
  }
  
  return NULL;
}


ccpc_t *ismodccpc(ccpc_t **predata, ccpc_t *nowdata, note_t *note, ccpc_t **head)
{
  int flg=0;
  ccpc_t *pre2, *ret=NULL;

  if (*predata == NULL) {
    *predata = nowdata;
    ret = nowdata;
  }
  else {
    pre2 = searchpre2ccpc(*predata);

    /* is-delete data because the same time */
    if (!cmppos((*predata)->pos, nowdata->pos)) {
      switch (config.ccpc.replace_ccpc) {
      case 0:
	break;
      case 1:
	ret = nowdata->prev;
	delccpc(nowdata, head);
	return ret;
      case 2:
	delccpc(*predata, head);
	*predata = nowdata;
	ret = nowdata;
	flg = 1;
	break;
      }
    }

    /* is-delete data because has declaired */
    if (config.ccpc.omit_ccpc == 1) {
      if (flg == 0 && !strcmp((*predata)->modvalue, nowdata->modvalue)) {
	ret = nowdata->prev;
	delccpc(nowdata, head);
	flg = 1;
      }
      else if (flg == 1 && pre2 != NULL &&
	       !strcmp(pre2->modvalue, nowdata->modvalue)) {
	ret = nowdata->prev;
	delccpc(nowdata, head);
	*predata = pre2;
	flg = 1;
      }
    }

    if (!flg) {
      *predata = nowdata;
      ret = nowdata;
    }
  }
  
  return ret;
}


int ispart(int n)
/* [return value of ispart]
 *  1 = FM part
 *  0 = SSG(PSG) part
 * -1 = out of printable range */
{
  switch(config.mml.mml_style) {
  case CUSTOM:
  case FMP7:
  case MXDRV:
    return ((n <= config.track.partsize)? 1 : -1);
  case FMP:
    if (config.track.print_partname == 1) {
      if ( n <= 3 || (6 < n && n <= config.track.partsize) ) {
	return 1;
      }
      else if (3 < n && n <= 6) {
	return 0;
      }
      else {
	return -1;
      }
    }
    else {
      return ((n <= config.track.partsize)? 1 : -1);
    }
    break;
  case PMD:
    if (config.track.print_partname == 1) {
      switch (config.track.pmd_autoname) {
      case 0:
	if (n <= 3) {
	  return 1;
	}
	else if (n <= config.track.partsize) {
	  return 0;
	}
	else {
	  return -1;
	}
      case 1:
	if (n <= 6) {
	  return 1;
	}
	else if (n <= config.track.partsize) {
	  return 0;
	}
	else {
	  return -1;
	}
      case 2:
	if ( n <= 6 || (9 < n && n <= config.track.partsize) ) {
	  return 1;
	}
	else if (6 < n && n <= 9) {
	  return 0;
	}
	else {
	  return -1;
	}
      case 3:
      case 4:
      case 5:
	return ((n <= config.track.partsize)? 1 : -1);
      }
    }
    else {
      return ((n <= config.track.partsize)? 1 : -1);
    }
    break;
  case NRTDRV:
    if (config.track.print_partname == 1) {
      switch (config.track.nrtdrv_autoname) {
      case 0:
	return ((n <= config.track.partsize)? 0 : -1);
      case 1:
	if (n <= 16) {
	  return 1;
	}
	else if (n <= config.track.partsize) {
	  return 0;
	}
	else {
	  return -1;
	}
	break;
      case 2:
	if ( n <= 8 || (11 < n && n <= config.track.partsize) ) {
	  return 1;
	}
	else if (8 < n && n <= 11) {
	  return 0;
	}
	else {
	  return -1;
	}
	break;
      }
    }
    else {
      return ((n <= config.track.partsize)? 1 : -1);
    }
    break;
  }

  /* return 0; */
}


char *convccpcvalue(ccpc_t *ccpc, int trackno)
{
  char *buf;

  buf = (char *)malloc(sizeof(char)*8);
  if (buf == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }
  
  switch (ccpc->type) {
    
  case VOLUME:
    switch (config.mml.mml_style) {
    case CUSTOM:
      if (config.ccpc.volume_range == 0) {
	sprintf(buf, "%s%u", config.ccpc.volume_cmd, ccpc->value);
      }
      else {
	convert_volume(buf, config.ccpc.volume_cmd, ccpc->value, config.ccpc.volume_range);
      }
      break;
    case FMP7:
      sprintf(buf, "v%u", ccpc->value);
      break;
    case FMP:
      convert_volume(buf, "v", ccpc->value, 15);
      break;
    case PMD:
      setvolume_PMD(buf, ccpc->value, trackno);
      break;
    case MXDRV:
      if (config.ccpc.volume_mxdrv == 0) {
	convert_volume(buf, "v", ccpc->value, 15);
      }
      else {
	sprintf(buf, "@v%u", ccpc->value);
      }
      break;
    case NRTDRV:
      setvolume_NRTDRV(buf, ccpc->value, trackno);
      break;
    }
    break;
    
  case PAN:
    switch (config.mml.mml_style) {
    case CUSTOM:
      if (config.ccpc.pan_cmdmode == 0) {
	sprintf(buf, "%s%u", config.ccpc.pan_midicmd, ccpc->value);
      }
      else {
	setpan_place(buf, ccpc->value);
      }
      break;
    case FMP7:
      setpan_placeFMP7(buf, ccpc->value);
      break;
    case FMP:
    case PMD:
    case MXDRV:
    case NRTDRV:
      setpan_place(buf, ccpc->value);
      break;
    }
    break;

  case PC:
    switch (config.mml.mml_style) {
    case CUSTOM:
      sprintf(buf, "%s%u", config.ccpc.pc_cmd, ccpc->value+1);
      break;
    case FMP7:
    case FMP:
    case PMD:
    case MXDRV:
    case NRTDRV:
      sprintf(buf, "@%u", ccpc->value+1);
      break;
    }
    break;

  }

  return buf;
}


void convert_volume(char *buf, char *cmd, unsigned char value, int range)
{
  unsigned int value1, value2;
  double tmp;

  tmp = (double)value * (double)range / 127.0;

  value1 = (unsigned int)tmp;
  value2 = value1 + 1;

  if (tmp - (double)value1 < (double)value2 - tmp) {
    sprintf(buf, "%s%u", cmd, value1);
  } else {
    sprintf(buf, "%s%u", cmd, value2);
  }

  return;
}


void setvolume_PMD(char *buf, unsigned char value, int trackno)
{
  switch (config.ccpc.volume_pmd) {
  case 0:
    if (config.track.print_partname == 1 && ispart(trackno) == 0) {
      convert_volume(buf, "v", value, 15);
    }
    else {
      convert_volume(buf, "v", value, 16);
    }
    break;
  case 1:
    if (config.track.print_partname == 1 && ispart(trackno) == 0) {
      convert_volume(buf, "V", value, 15);
    }
    else {
      sprintf(buf, "V%u", value);
    }
    break;
  }
 
  return;
}


void setvolume_NRTDRV(char *buf, unsigned char value, int trackno)
{
  switch (config.ccpc.volume_nrtdrv) {
  case 0:
    if (config.track.print_partname == 1 && ispart(trackno) == 0) {
      convert_volume(buf, "v", value, 15);
    }
    else {
      convert_volume(buf, "v", value, config.ccpc.vstep_nrtdrv);
    }
    break;
  case 1:
    sprintf(buf, "V%u", value);
    break;
  }
 
  return;
}


void setpan_placeFMP7(char *buf, unsigned char value)
{
  int flg;
  unsigned int value1, value2, pvalue;
  double tmp;

  if (value == 64) {   // center
    flg = 2;
    pvalue = 128;
  }
  else {
    if (value > 64) {   // right
      flg = 0;
      tmp = value - 64;
      tmp = tmp * 127 / 63;
    }
    else {   // left
      flg = 1;
      tmp = 64 - value;
      tmp = tmp * 127 / 64;
    }
    value1 = (unsigned int)tmp;
    value2 = value1 + 1;
    pvalue = (tmp - (double)value1 < (double)value2 - tmp)? value1 : value2;
  }
  
  if (config.ccpc.pan_fmp7 == 0) {
    switch (flg) {
    case 0:
      pvalue = 128 + pvalue;
      break;
    case 1:
      pvalue = 128 - pvalue;
      break;
    case 2:
      break;
    }
    sprintf(buf, "P%u", pvalue);
  }
  else {
    switch (flg) {
    case 0:
      sprintf(buf, "%s%u", config.ccpc.pan_rcmd, pvalue);
      break;
    case 1:
      sprintf(buf, "%s%u", config.ccpc.pan_lcmd, pvalue);
      break;
    case 2:
      sprintf(buf, "%s", config.ccpc.pan_ccmd);
      break;
    }
  }

  return;
}


void setpan_place(char *buf, unsigned char value)
{
  if (value < config.ccpc.pan_left) {
    sprintf(buf, "%s", config.ccpc.pan_lcmd);
  }
  else if (config.ccpc.pan_right < value) {
    sprintf(buf, "%s", config.ccpc.pan_rcmd);
  }
  else {
    sprintf(buf, "%s", config.ccpc.pan_ccmd);
  }

  return;
}


ccpc_t *delccpc(ccpc_t *del, ccpc_t **head)
{
  ccpc_t *pre, *next;
  
  pre = del->prev;
  next = del->next;
  if (pre == NULL) {   // in head
    next->prev = NULL;
    *head = next;
  }
  else if (next == NULL) {   // in rear
    pre->next = NULL;
  }
  else {
    pre->next = next;
    next->prev = pre;
  }
  FREE(del->modvalue);
  FREE(del);

  return next;
}


void modify_note_and_rest_by_ccpc(track_t *track)
{
  note_t *note=track->note_head;
  ccpc_t *ccpc=track->ccpc_head;

  while (note != NULL && ccpc != NULL) {
    
    if (!cmppos(note->pos, ccpc->pos)) {   // the same time
      ccpc = ccpc->next;
    }
    else if (!cmppos(note->endpos, ccpc->pos)) {   // the same time (for last of track)
      note = note->next;
      ccpc = ccpc->next;
    }
    else if (cmppos(note->pos, ccpc->pos) < 0 &&
	     cmppos(note->endpos, ccpc->pos) > 0) {   // overlap
      if (note->velocity > 0) {   // when note
	switch (config.ccpc.overlap_ccpc) {
	case 0:
	  note = make_cutnote(note, ccpc->pos, 1);
	  break;
	case 1:
	case 2:
	  fprintf(stderr, " >> ノートが他のイベントと重複しています。 トラック no.%d 小節:%u tick:%u ", track->num, ccpc->pos.measure, ccpc->pos.miditick);
	  switch (ccpc->type) {
	  case VOLUME:
	    fprintf(stderr, "[control change: volume] value:%u\n", ccpc->value);
	    break;
	  case PAN:
	    fprintf(stderr, "[control change: pan] value:%u\n", ccpc->value);
	    break;
	  case PC:
	    fprintf(stderr, "[program change] no:%u\n", ccpc->value);
	    break;
	  }
	  if (config.ccpc.overlap_ccpc == 1) {
	    delccpc(ccpc, &track->ccpc_head);   // delete overlapping ccpc
	  }
	  ccpc = ccpc->next;
	  break;
	}
      }
      else {      // when rest, cut rest to insert ccpc
	note = make_cutnote(note, ccpc->pos, 1);
      }

    }
    else {   // other time
      note = note->next;
    }

  }

  return;
}


note_t *make_cutnote(note_t *prenote, pos_t inpos, int tieflgtype)
{
  note_t *newnote, *nextnote;

  newnote = (note_t *)malloc(sizeof(note_t));
  if (newnote == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }
  newnote->pos = inpos;
  newnote->endpos = prenote->endpos;
  newnote->velocity = prenote->velocity;
  newnote->key = prenote->key;
  newnote->prev = prenote;
  nextnote = prenote->next;
  newnote->next = nextnote;
  
  prenote->endpos = inpos;
  prenote->next = newnote;

  if (tieflgtype == 0) {   // cut by measure
    newnote->tieflg = 1;
    prenote->tieflg += 3;
  }
  else {   // cut by ccpc
    newnote->tieflg = 2;
    prenote->tieflg += 6;
  }

  if (nextnote != NULL) {
    nextnote->prev = newnote;
  }

  return newnote;
}


elist_t **create_eventlists(track_t *track)
{
  int i;
  elist_t **seq;

  seq = (elist_t **)malloc(sizeof(elist_t *)*config.track.tracksize);
  if (seq == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }
  
  for (i=0; i<config.track.tracksize; i++) {
    seq[i] = sort_events(track);
    track = track->next;
  }

  return seq;
}


elist_t *sort_events(track_t *track)
{
  note_t *note=track->note_head;
  ccpc_t *ccpc=track->ccpc_head;
  marker_t *marker=marker_head;
  tempo_t *tempo=tempo_head;
  rhythm_t *rhythm=rhythm_head;
  keysig_t *keysig=keysig_head;
  elist_t *head, *rear;
  
  head = make_elist(RHYTHM, rhythm);
  rear = head;
  rhythm = rhythm->next;
    
  for ( ; rhythm!=NULL; rhythm=rhythm->next) {
    rear = INSERT_LIST(RHYTHM, rhythm);
  }
  for ( ; marker!=NULL; marker=marker->next) {
    rear = INSERT_LIST(MARKER, marker);
  }
  for ( ; tempo!=NULL; tempo=tempo->next) {
    rear = INSERT_LIST(TEMPO, tempo);
  }
  for ( ; keysig!=NULL; keysig=keysig->next) {
    rear = INSERT_LIST(KEYSIG, keysig);
  }
  for ( ; ccpc!=NULL; ccpc=ccpc->next) {
    rear = INSERT_LIST(CCPC, ccpc);
  }
  for ( ; note!=NULL; note=note->next) {
    rear = INSERT_LIST(NOTE, note);
  }

  return head;
}


elist_t *make_elist(enum _LISTTYPE type, void *data)
{
  elist_t *new;
    
  new = (elist_t *)malloc(sizeof(elist_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }

  new->type = type;
  
  switch (type) {
  case RHYTHM:
    new->rhythm = (rhythm_t *)data;
    new->marker = NULL;
    new->tempo = NULL;
    new->keysig = NULL;
    new->ccpc = NULL;
    new->note = NULL;
    break;
  case MARKER:
    new->rhythm = NULL;
    new->marker = (marker_t *)data;
    new->tempo = NULL;
    new->keysig = NULL;
    new->ccpc = NULL;
    new->note = NULL;
    break;
  case TEMPO:
    new->rhythm = NULL;
    new->marker = NULL;
    new->tempo = (tempo_t *)data;
    new->keysig = NULL;
    new->ccpc = NULL;
    new->note = NULL;
    break;
  case KEYSIG:
    new->rhythm = NULL;
    new->marker = NULL;
    new->tempo = NULL;
    new->keysig = (keysig_t *)data;
    new->ccpc = NULL;
    new->note = NULL;
    break;
  case CCPC:
    new->rhythm = NULL;
    new->marker = NULL;
    new->tempo = NULL;
    new->keysig = NULL;
    new->ccpc = (ccpc_t *)data;
    new->note = NULL;
    break;
  case NOTE:
    new->rhythm = NULL;
    new->marker = NULL;
    new->tempo = NULL;
    new->keysig = NULL;
    new->ccpc = NULL;
    new->note = (note_t *)data;
    break;
  }

  new->prev = NULL;
  new->next = NULL;
  
  return new;
}


pos_t getoutlistpos(elist_t *data)
{
  switch (data->type) {
  case RHYTHM: return data->rhythm->pos;
  case MARKER: return data->marker->pos;
  case TEMPO:  return data->tempo->pos;
  case KEYSIG: return data->keysig->pos;
  case CCPC:   return data->ccpc->pos;
  case NOTE:   return data->note->pos;
  }

  /* return; */
}


elist_t *insert_outlist(elist_t **head_p, elist_t *cmp, elist_t *new)
{
  elist_t *nextcmp=NULL, *rear=cmp;
  pos_t new_pos, cmp_pos;

  new_pos = getoutlistpos(new);
    
  while (cmp != NULL) {
    cmp_pos = getoutlistpos(cmp);
    if ( cmppos(cmp_pos, new_pos) < 0 ||
	 (!cmppos(cmp_pos, new_pos) && cmp->type <= new->type) ) {
      break;
    }
    nextcmp = cmp;
    cmp = cmp->prev;
  }   // while (cmp)
    
  if (cmp == NULL) {   // in head
    *head_p = new;
  }
  else {
    cmp->next = new;
    new->prev = cmp;
  }
  if (nextcmp == NULL) {   // in rear
    rear = new;
  }
  else {
    new->next = nextcmp;
    nextcmp->prev = new;
  }

  
  return rear;
}


void free_event_list(elist_t **seq)
{
  int i;
  
  for (i=0; i<config.track.tracksize; i++) {
    free_events(seq[i]);
  }
  FREE(seq);
  
  return;
}


void free_events(elist_t *head)
{
  elist_t *tmp;

  while (head != NULL) {
    tmp = head;
    head = head->next;
    FREE(tmp);
  }

  return;
}


unsigned int isinsert_tempo(elist_t **seq)
{
  int i;
  int noteflg, ronlyflg, overlaptempoflg=0;   // for check tempo
  elist_t *cmp;
  pos_t noteendpos, tempopos;

  
  for (i=0; i<config.track.tracksize; i++) {

    if ( !(config.track.print_track == 2 && !istrack(i+1)) ) {

      cmp = seq[i];
      noteflg = 0;
      ronlyflg = 1;
      overlaptempoflg = 0;

      for ( ; cmp!=NULL; cmp=cmp->next) {
    
	switch (cmp->type) {
	case RHYTHM:
	case MARKER:
	case KEYSIG:
	case CCPC:
	  break;
	case TEMPO:
	  if (noteflg == 1) {
	    noteflg = 2;
	    tempopos = cmp->tempo->pos;
	  }
	  break;
	case NOTE:
	  if (cmp->note->velocity > 0) {
	    noteendpos = cmp->note->endpos;
	    noteflg = 1;
	    ronlyflg = 0;
	  }
	  else {
	    noteflg = 0;
	  }
	  break;
	}
	
	if (noteflg == 2) {
	  if (config.tempo.tempo_enable == 1 &&
	      cmppos(noteendpos, tempopos) > 0) {   // tempo overlaps note
	    overlaptempoflg = 1;
	    break;
	  }
	  else {
	    noteflg = 1;
	  }
	}
      
      }   // for (cmp)
    

      if (ronlyflg == 0 && overlaptempoflg == 0) {
	return i + 1;
      }

    }   // if (!(config.track))
      
  }   // for (i)

  
  switch (config.tempo.make_tempopart) {
  case 0:
    fprintf(stderr, " >> テンポコマンドを挿入する場所がありませんでした。\n");
    break;
  case 1:
    puts(" >> テンポパートを作成します。");
    break;
  case 2:
    break;
  }

  
  return 0;
}


int istrack(unsigned int num)
{
  int i;
  
  for (i=0; config.track.number_track[i]!=0; i++) {
    if (config.track.number_track[i] == num) {
      return 1;
    }
  }

  return 0;
}


elist_t **modify_tempopart(elist_t **seq, unsigned int *t_partnum)
{
  int i=0;
  track_t track;   // only this scope
  pos_t pos;
  tempo_t *place;
  note_t *head, *rest;

  if (*t_partnum == 0) {   // make tempopart
    switch (config.tempo.make_tempopart) {
    case 0:
      break;
    case 1:  
      head = NULL;
      place = tempo_head;
      pos = place->pos;
      rest = make_note_list(head, pos, &head);
      for ( ; place->next!=NULL; place=place->next) {
	rest->endpos = place->pos;
	rest->velocity = 0;
	rest = make_note_list(rest, place->pos, &head);
      }
      rest->endpos = totalpos;
      rest->velocity = 0;
      track.note_head = head;
      track.ccpc_head = NULL;
      t_part = sort_events(&track);
      cut_tempopart(t_part);
      break;
    case 2:
      for (i=0; seq[i]==NULL; i++)
	;
      cut_tempopart(seq[i]);
      *t_partnum = i + 1;
      break;
    }
  }
  else {
    cut_tempopart(seq[(*t_partnum)-1]);
  }

  return seq;
}


void cut_tempopart(elist_t *track)
{
  int restflg;
  elist_t *cmp, *rest;
  pos_t tempopos;
  
  cmp = track;
  restflg = 0;

  for ( ; cmp!=NULL; cmp=cmp->next) {
    switch (cmp->type) {
    case RHYTHM:
    case MARKER:
    case KEYSIG:
    case CCPC:
      break;
    case TEMPO:
      if (restflg == 1) {
	restflg = 2;
	tempopos = cmp->tempo->pos;
      }
      break;
    case NOTE:
      if (cmp->note->velocity == 0 || config.tempo.make_tempopart == 2) {
	restflg = 1;
	rest = cmp;
      }
      else {
	restflg = 0;
      }
      break;
    }

    if (restflg == 2) {
      if (cmppos(rest->note->endpos, tempopos) > 0) {   // tempo overlaps rest
	cut_and_sort_note(rest, cmp, cmp->tempo->pos, 1);
      }
      restflg = 1;
    }

  }   // for(cmp)

  return;
}


void cut_and_sort_note(elist_t *note, elist_t *preinevent, pos_t in_pos, int tempocutflg)
{
  elist_t *newnote, *cmp, *precmp;
  pos_t cmp_pos;
  int lastflg=0;

  cmp = preinevent;

  do {
    if (cmp->next == NULL) {
      lastflg = 1;
      break;
    }
    cmp = cmp->next;
    cmp_pos = getoutlistpos(cmp);
  } while (cmppos(in_pos, cmp_pos) >= 0);

  if (tempocutflg == 0) {   // cut by measure
    newnote = make_elist(NOTE, make_cutnote(note->note, in_pos, 0));
  }
  else {   // cut by tempo
    newnote = make_elist(NOTE, make_cutnote(note->note, in_pos, 1));
  }

  if (lastflg == 1) {
    newnote->next = NULL;
    newnote->prev = cmp;
    cmp->next = newnote;
  }
  else {
    newnote->prev = cmp->prev;
    newnote->next = cmp;
    precmp = cmp->prev;
    cmp->prev = newnote;
    precmp->next = newnote;
  }
  
  return;
}


elist_t **cut_note_by_measure(elist_t **seq, unsigned int t_partnum)
{
  int i;
  
  for (i=0; i<config.track.tracksize; i++) {
    search_cut_note(seq[i]);
  }

  if (t_partnum == 0) {
    search_cut_note(t_part);
  }

  return seq;
}


void search_cut_note(elist_t *cmp)
{
  pos_t pos, endpos;
  rhythm_t *rhythm=NULL;
    
  for ( ; cmp!=NULL; cmp=cmp->next) {
    
    switch (cmp->type) {
    case RHYTHM:
      rhythm = cmp->rhythm;
      break;
    case NOTE:
      if ( (config.note.measurecut_note == 2) ||
	   (config.note.measurecut_note == 1 && cmp->note->velocity == 0) ) {
	pos = cmp->note->pos;
	endpos = cmp->note->endpos;
	if ( (pos.measure + 1 < endpos.measure) ||
	     (pos.measure + 1 == endpos.measure && endpos.miditick > 0) ) {
	  pos.measure++;
	  pos.miditick = 0;
	  pos.mmltick = 0;
	  cut_and_sort_note(cmp, cmp, pos, 0);
	}
      }
      break;
    default:
      break;
    }

  }

  return;
}
