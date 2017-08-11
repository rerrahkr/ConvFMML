#include <string.h>
#include "convfmml.h"
#include "analyze.h"


track_t *func_analyze(struct _CHUNK *chunkdata, unsigned short num)
{
  int i;
  track_t *head=NULL, *track=NULL;

  /*--- init meta-events ---*/
  make_marker_list(marker_head, totalpos, NULL, 0);
  make_tempo_list(tempo_head, totalpos);
  tempo_head->data = 500000;   // 120bpm
  setrhythmdata(rhythm_head, totalpos, totalpos, 4, 2, 24, 8);
  make_keysig_list(keysig_head, totalpos, 0, 0);

  /*--- analyze MIDI data ---*/
  for (i=0; i<num; i++) {
    track = analyze_chunkdata(chunkdata[i], track, &head, i+1);
  }

  return head;
}


track_t *analyze_chunkdata(const struct _CHUNK chunkdata, track_t *track, track_t **head, int num)
{
  int i;
  byte_t b;

  ccpc_t *ccpc=NULL;
  marker_t *marker=marker_head;
  tempo_t *tempo=tempo_head;
  rhythm_t *rhythm=rhythm_head;
  keysig_t *keysig=keysig_head;
  track_t *mk;
  
  unsigned int delta;
  unsigned int datasize;
  byte_t status;
  pos_t oldpos;
  pos_t pos={
    .measure=1,
    .miditick=0,
    .mmltick=0
  };

  int errflg=0;


  /*--- create trackdata1 ---*/
  track = addtrackcell(track, head, num);

  /*--- analyze binary data ---*/
  for (i=0; i<chunkdata.size; i++) {
    
    
    /*--- calculate delta-time and skip byte ---*/
    i += calcVLQshiftbyte(&chunkdata.event[i], &delta);


    /*--- record current position ---*/
    pos = calcpos(pos, delta, &rhythm);
    if (cmppos(pos, totalpos) > 0) {
      totalpos = pos;
    }
    
    
    /*--- check running status ---*/
    if ( (b = chunkdata.event[i]) & 0x80) {
      status = b;
      i++;   // skip byte
    }
    
    
    /*--- judge event (current byte position is 2nd byte of event data) ---*/
    switch (status & 0xf0) {

      /*=== note off ===*/
    case 0x80:
	setnoteoff(track, pos, status&0x0f, chunkdata.event[i]);
	i++;   // skip byte
      break;
      /*=== note on ==*/
    case 0x90:
      if (chunkdata.event[i+1] == 0x00) {   // 0x90 note off
	setnoteoff(track, pos, status&0x0f, chunkdata.event[i]);
      }
      else {   // ordinary process
	setnoteon(track, pos, status&0x0f, chunkdata.event[i], chunkdata.event[i+1], head, num);
      }
      i++;   // skip byte
      break;
      
      /*=== control change ===*/
    case 0xb0:
      b = chunkdata.event[i++];
      if (config.ccpc.volume_enable == 1 && b == 0x07) {   // volume change
	ccpc = setccpcdata(ccpc, pos, VOLUME, status&0x0f, chunkdata.event[i], &track->ccpc_head);
      }
      else if (config.ccpc.pan_enable == 1 && b == 0x0a) {   // pan change
	ccpc = setccpcdata(ccpc, pos, PAN, status&0x0f, chunkdata.event[i], &track->ccpc_head);
      }
      break;

      /*=== program change ===*/
    case 0xc0:
      if (config.ccpc.pc_enable == 1) {
	ccpc = setccpcdata(ccpc, pos, PC, status&0x0f, chunkdata.event[i], &track->ccpc_head);
      }
      break;

      /*=== polyphonic key pressure ===*/
    case 0xa0:
      i += 1;   // skip byte
      break;

      /*=== channel pressure ===*/
    case 0xd0:
      break;

      /*=== pitch bend ===*/
    case 0xe0:
      i += 1;   // skip byte
      break;

      /*=== other event ===*/
    case 0xf0:
      switch (status) {

	/*=== sysex event ===*/
      case 0xf0:
      case 0xf7:
	calcVLQshiftbyte(&chunkdata.event[i], &datasize);
	i += datasize;   // skip byte
	break;

	/*=== meta event ===*/
      case 0xff:
	b = chunkdata.event[i];
	i++;   // skip byte
	switch (b) {

	  /*=== marker ===*/
	case 0x06:
	  calcVLQshiftbyte(&chunkdata.event[i], &datasize);
	  marker = make_marker_list(marker, pos, &chunkdata.event[i+1], datasize);
	  i += datasize;   // skip byte
	  break;

	  /*=== tempo ===*/
	case 0x51:
	  i++;   // skip byte
	  tempo = settempodata(tempo, pos, &chunkdata.event[i]);
	  i += 2;   // skip byte
	  break;

	  /*=== rhythm ===*/
	case 0x58:
	  i++;   // skip byte
	  oldpos = pos;   // for comparing pre-cell
	  if (pos.miditick > 0) {
	    pos.measure++;
	    pos.miditick = 0;
	    pos.mmltick = 0;
	  }
	  if (cmppos(pos, totalpos) > 0) {
	    totalpos = pos;
	  }
	  rhythm = setrhythmdata(rhythm, oldpos, pos, chunkdata.event[i], chunkdata.event[i+1], chunkdata.event[i+2], chunkdata.event[i+3]); 
	  /* modify position data */
	  if (cmppos(pos, oldpos)) {
	    if (!cmppos(marker->pos, oldpos)) {
	      marker->pos = pos;
	    }
	    if (!cmppos(tempo->pos, oldpos)) {
	      tempo->pos = pos;
	    }
	    if (!cmppos(keysig->pos, oldpos)) {
	      keysig->pos = pos;
	    }
	    for (mk=track; mk!=NULL; mk=mk->next) {
	      ismodify_notepos(mk->note_now, oldpos, pos);
	    }
	    ismodify_ccpcpos(ccpc, oldpos, pos);
	  }
	  i += 3;   // skip byte
	  break;

	  /*=== key signature ===*/
	case 0x59:
	  i++;   // skip byte
	  keysig = make_keysig_list(keysig, pos, chunkdata.event[i], chunkdata.event[i+1]);
	  i++;   // skip byte
	  break;

	  /*=== end of track ===*/
	case 0x2f:
	  for (mk=track; mk!=NULL; mk=mk->next) {
	    if (!cmppos(mk->note_now->pos, pos) && mk->note_now->prev != NULL) {   // delete last note list
	      delnote(mk->note_now, &mk->note_head);
	    }
	    else {   // make rest data-list
	      mk->note_now->endpos = pos;
	      mk->note_now->velocity = 0;
	    }
	  }
	  break;
	  
	  /*=== other meta event ===*/
	case 0x01:
	case 0x02:
	case 0x03:
	case 0x04:
	case 0x05:
	case 0x07:
	case 0x08:
	case 0x09:
	case 0x7f:
	  calcVLQshiftbyte(&chunkdata.event[i], &datasize);
	  i += datasize;   // skip byte
	  break;
	case 0x20:
	case 0x21:
	  i += 1;   // skip byte
	  break;
	case 0x54:
	  i += 5;   // skip byte
	  break;
	  
	default:
	  errflg = 1;
	  break;
	  
	}   // switch (b)
	break;
	
      default:
	errflg = 1;
	break;
	
      }   // switch (status)
      break;

    default:
      errflg = 1;
      break;
    }   // switch (status & 0x80)

    
    if (errflg == 1) {
      fprintf(stderr, "[ERROR] データが壊れています。\n");
      free_metalists();
      free_config();
      ERREXIT();
    }

    
  }   // for (chunkdata.size)


  /*--- set eotpos-data to track compositions ---*/
  mk = track;
  while (1) {
    mk->eotpos = pos;
    if (mk != track) {
      mk->ccpc_head = copyccpclist(track->ccpc_head);
    }
    if (mk->next == NULL) {
      break;
    }
    mk = mk->next;
  }

  
  return mk;
}


track_t *addtrackcell(track_t *pre, track_t **head, int num)
{
  track_t *new;
  pos_t pos={
    .measure=1,
    .miditick=0,
    .mmltick=0
  };

  new = (track_t *)malloc(sizeof(track_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }

  new->num = num;
  new->note_head = NULL;
  new->note_now = make_note_list(NULL, pos, &new->note_head);
  new->ccpc_head = NULL;
  new->next = NULL;

  if (*head == NULL) {
    new->prev = NULL;
    *head = new;
  }
  else {
    pre->next = new;
    new->prev = pre;
  }

  return new;
}


track_t *deltrackcell(track_t *del, track_t **head)
{
  track_t *pre, *next;

  pre = del->prev;
  next = del->next;

  if (pre == NULL) {
    next->prev = NULL;
    *head = next;
  }
  else if (next == NULL) {
    pre->next = NULL;
  }
  else {
    pre->next = next;
    next->prev = pre;
  }

  free_note_list(del->note_head);
  free_ccpc_list(del->ccpc_head);
  FREE(del);

  return next;
}


void free_track_list(track_t *list)
{
  track_t *p;

  while (list != NULL) {
    p = list;
    list = list->next;
    free_note_list(p->note_head);
    free_ccpc_list(p->ccpc_head);
    FREE(p);
  }

  return;
}


int calcVLQshiftbyte(byte_t *byte, unsigned int *VLQ)
{
  byte_t b;
  int shift=0;

  *VLQ = 0;

  while ( (b = *(byte + shift++)) & 0x80 ) {
    *VLQ |= (b & 0x7f);
    *VLQ <<= 7;
  }
  *VLQ |= b;
  
  return shift;
}


int cmppos(pos_t a, pos_t b)
{
  unsigned int atick=a.mmltick;
  unsigned int btick=b.mmltick;
  
  if (a.measure > b.measure) {
    return 2;
  }
  else if (a.measure == b.measure) {
    if (atick > btick) {
      return 1;
    }
    else if (atick == btick) {
      return 0;
    }
    else {   // atick < btick
      return -1;
    }
  }
  else {   // a.measure < b.measure
    return -2;
  }
}


pos_t calcpos(pos_t pos, unsigned int delta, rhythm_t **rhythm)
{
  pos.miditick += delta;

  if ((*rhythm)->endpos.measure == 0) {   // while read condacor track

    while (1) {
      if (pos.miditick >= (*rhythm)->midimes_tick) {
	pos.measure++;
	pos.miditick -= (*rhythm)->midimes_tick;
      }
      else {
	pos.mmltick = calcmmltick(pos.miditick);
	return pos;
      }
    }
    
  }
  else {
  
    while (1) {
      switch (cmppos(pos, (*rhythm)->endpos)) {
      case 1:
      case 0:
	if ((*rhythm)->endpos.miditick > 0) {
	  pos.measure++;
	  pos.miditick -= (*rhythm)->endpos.miditick;
	  pos.mmltick = calcmmltick(pos.miditick);
	}
	(*rhythm) = (*rhythm)->next;
	break;
      case -1:
	pos.mmltick = calcmmltick(pos.miditick);
	return pos;
      default:
	if (pos.miditick >= (*rhythm)->midimes_tick) {
	  pos.measure++;
	  pos.miditick -= (*rhythm)->midimes_tick;
	  pos.mmltick = calcmmltick(pos.miditick);
	}
	else {
	  pos.mmltick = calcmmltick(pos.miditick);
	  return pos;
	}
	break;
      }
    }
  
  }

  /* return pos; */
}


unsigned int calcmmltick(unsigned int miditick)
{
  double exact, rndup, rndoff;

  exact = miditick * tbratio;
  rndoff = (double)((unsigned int)exact);
  rndup = rndoff + 1.0;

  if (rndup - exact > exact - rndoff) {   // rounding
    return (unsigned int)rndoff;
  }
  else {
    return (unsigned int)rndup;
  }
  
  /* return 0; */
}


marker_t *make_marker_list(marker_t *pre, pos_t pos, byte_t *data, unsigned int size)
{  
  marker_t *new, *del;
  
  new = (marker_t *)malloc(sizeof(marker_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }

  new->pos = pos;
  new->next = NULL;
  
  if (pre == NULL) {   // in head
    new->prev = NULL;
    marker_head = new;
  }
  else {
    if (!cmppos(pre->pos, pos)) {
      del = pre;
      pre = pre->prev;
      free_marker_list(del);
    }
    if (pre == NULL) {   // in head (foward del-list)
      new->prev = NULL;
      marker_head = new;
    }
    else {
      pre->next = new;
      new->prev = pre;
    }
  }

  if (size == 0) {
    new->data = NULL;
  }
  else {
    new->data = (unsigned char *)malloc(sizeof(unsigned char)*(size+1));
    if (new->data == NULL) {
      EPRINTM();
      free_metalists();
      free_config();
      ERREXIT();
    } 
    memcpy(new->data, data, size);
    new->data[size] = '\0';
  }
  
  return new;
}


void free_marker_list(marker_t *list)
{
  marker_t *tmp;
  
  while (list != NULL) {
    FREE(list->data);
    tmp = list;
    list = list->next;
    FREE(tmp);
  }

  return;
}


tempo_t *make_tempo_list(tempo_t *prelist, pos_t pos)
{
  tempo_t *new;
  tempo_t *del;

  new = (tempo_t *)malloc(sizeof(tempo_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  } 

  new->pos = pos;
  new->next=NULL;
  
  if (prelist == NULL) {   // in head
    new->prev = NULL;
    tempo_head = new;
  }
  else {
    if (!cmppos(prelist->pos, pos)) {
      del = prelist;
      prelist = prelist->prev;
      free_tempo_list(del);
    }
    if (prelist == NULL) {   // in head (foward del-list)
      new->prev = NULL;
      tempo_head = new;
    }
    else {
      prelist->next = new;
      new->prev = prelist;
    }
  }
  
  return new;
}


void free_tempo_list(tempo_t *list)
{
  tempo_t *tmp;

  while (list != NULL) {
    tmp = list;
    list = list->next;
    FREE(tmp);
  }

  return;
}


tempo_t *settempodata(tempo_t *tempo, pos_t pos, byte_t *data)
{
  int i;
  
  tempo = make_tempo_list(tempo, pos);
  tempo->data = 0;
  for (i=0; i<3; i++) {
    tempo->data <<= 8;
    tempo->data |= *(data + i);
  }  

  return tempo;
}


rhythm_t *make_rhythm_list(rhythm_t *prelist, pos_t pos)
{
  rhythm_t *new;
  rhythm_t *del;

  new = (rhythm_t *)malloc(sizeof(rhythm_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  } 

  new->next=NULL;
  
  if (prelist == NULL) {   // in head
    new->prev = NULL;
    rhythm_head = new;
  }
  else {
    if (!cmppos(prelist->pos, pos)) {
      del = prelist;
      prelist = prelist->prev;
      free_rhythm_list(del);
    }
    if (prelist == NULL) {   // in head (foward del-list)
      new->prev = NULL;
      rhythm_head = new;
    }
    else {
      prelist->next = new;
      new->prev = prelist;
    }
  }

  return new;
}


void free_rhythm_list(rhythm_t *list)
{
  rhythm_t *tmp;

  while (list != NULL) {
    tmp = list;
    list = list->next;
    FREE(tmp);
  }

  return;
}


rhythm_t *setrhythmdata(rhythm_t *rhythm, pos_t oldpos, pos_t newpos, byte_t num, byte_t denom_pow2, byte_t beatclock, byte_t hdm_semi)
{
  if (rhythm != NULL) {
    rhythm->endpos = oldpos;   // for searching note
  }
  rhythm = make_rhythm_list(rhythm, oldpos);
  rhythm->pos = newpos;
  rhythm->endpos.measure = 0;   // not set flag
  rhythm->num = num;
  rhythm->denom_pow2 = denom_pow2;
  rhythm->beatclock = beatclock;
  rhythm->hdm_semi = hdm_semi;
  rhythm->midimes_tick = (timebase >> rhythm->denom_pow2) * rhythm->num;
  rhythm->mmlmes_tick = calcmmltick(rhythm->midimes_tick);

  return rhythm;
}


keysig_t *make_keysig_list(keysig_t *prelist, pos_t pos, byte_t sig_num, byte_t minorflg)
{
  keysig_t *new;
  keysig_t *del;

  new = (keysig_t *)malloc(sizeof(keysig_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  } 

  new->pos = pos;
  new->sig_num = sig_num;
  new->minorflg = minorflg;
  new->next=NULL;
  
  if (prelist == NULL) {   // in head
    new->prev = NULL;
    keysig_head = new;
  }
  else {
    if (!cmppos(prelist->pos, pos)) {
      del = prelist;
      prelist = prelist->prev;
      free_keysig_list(del);
    }
    if (prelist == NULL) {   // in head (foward del-list)
      new->prev = NULL;
      keysig_head = new;
    }
    else {
      prelist->next = new;
      new->prev = prelist;
    }
  }

  return new;
}


void free_keysig_list(keysig_t *list)
{
  keysig_t *tmp;

  while (list != NULL) {
    tmp = list;
    list = list->next;
    FREE(tmp);
  }

  return;
}


void free_metalists(void)
{
  free_marker_list(marker_head);
  free_tempo_list(tempo_head);
  free_rhythm_list(rhythm_head);
  free_keysig_list(keysig_head);
  
  return;
}


note_t *make_note_list(note_t *prelist, pos_t pos, note_t **head)
{
  note_t *new;
  note_t *del;

  new = (note_t *)malloc(sizeof(note_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  } 

  new->tieflg = 0;
  new->pos = pos;
  new->endpos.measure = 0;
  new->velocity = 0;
  new->next=NULL;
  
  if (prelist == NULL) {   // in head
    new->prev = NULL;
    *head = new;
  }
  else {
    if (!cmppos(prelist->pos, pos)) {
      del = prelist;
      prelist = prelist->prev;
      free_note_list(del);
    }
    if (prelist == NULL) {   // in head (foward del-list)
      new->prev = NULL;
      *head = new;
    }
    else {
      prelist->next = new;
      new->prev = prelist;
    }
  }

  return new;
}


note_t *delnote(note_t *del, note_t **head)
{
  note_t *pre, *next;

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
  FREE(del);

  return pre;
}


void free_note_list(note_t *list)
{
  note_t *tmp;
  int i=0;
  
  while (list != NULL) {
    tmp = list;
    list = list->next;
    FREE(tmp);
  }

  return;
}


track_t *setnoteon(track_t *track, pos_t pos, byte_t ch, byte_t key, byte_t velocity, track_t **head, int num)
{
  while (track->note_now->velocity > 0) {
    if (track->next == NULL) {
      track = addtrackcell(track, head, num);
      break;
    }
    track = track->next;
  }

  if (cmppos(pos, track->note_now->pos) > 0) {   // set rest-off
    track->note_now->endpos = pos;
    track->note_now->velocity = 0;
    track->note_now = make_note_list(track->note_now, pos, &track->note_head);   // make note cell
  }

  track->note_now->ch = ch;
  track->note_now->key = key;
  track->note_now->velocity = velocity;

  return track;
}


track_t *setnoteoff(track_t *track, pos_t pos, byte_t ch, byte_t key)
{
  while (track->note_now->ch != ch || track->note_now->key != key) {
    track = track->next;
  }
  track->note_now->endpos = pos;
  track->note_now = make_note_list(track->note_now, pos, &track->note_head);   // make note-rest cell

  return track;
}


ccpc_t *make_ccpc_list(ccpc_t *prelist, pos_t pos, ccpc_t **head)
{
  ccpc_t *new;
  ccpc_t *del;

  new = (ccpc_t *)malloc(sizeof(ccpc_t));
  if (new == NULL) {
    EPRINTM();
    free_metalists();
    free_config();
    ERREXIT();
  }

  new->pos = pos;
  new->modvalue=NULL;
  new->next=NULL;
  
  if (prelist == NULL) {   // in head
    new->prev = NULL;
    *head = new;
  }
  else {
    prelist->next = new;
    new->prev = prelist;
  }

  return new;
}


void free_ccpc_list(ccpc_t *list)
{
  ccpc_t *tmp;

  while (list != NULL) {
    tmp = list;
    list = list->next;
    FREE(tmp->modvalue);
    FREE(tmp);
  }

  return;
}


ccpc_t *copyccpclist(ccpc_t *src)
{
  ccpc_t *head=NULL;
  ccpc_t *now=NULL;

  for ( ; src!=NULL; src=src->next) {
    now = make_ccpc_list(now, src->pos, &head);
    now->type = src->type;
    now->ch = src->ch;
    now->value = src->value;
  }

  return head;
}


ccpc_t *setccpcdata(ccpc_t *ccpc, pos_t pos, enum _CCPCTYPE type, byte_t ch, byte_t value, ccpc_t **head)
{
  ccpc = make_ccpc_list(ccpc, pos, head);
  ccpc->type = type;
  ccpc->ch = ch;
  ccpc->value = value;

  return ccpc;
}


void ismodify_notepos(note_t *cmp, pos_t oldpos, pos_t newpos)
{
  if (!cmppos(cmp->pos, oldpos)) {
    cmp->pos = newpos;
    cmp = cmp->prev;
    if (cmp != NULL) {
      cmp->endpos = newpos;
    }
  }
    
  return;
}


void ismodify_ccpcpos(ccpc_t *cmp, pos_t oldpos, pos_t newpos)
{
  while (cmp != NULL && !cmppos(cmp->pos, oldpos)) {
    cmp->pos = newpos;
    cmp = cmp->prev;
  }

  return;
}
