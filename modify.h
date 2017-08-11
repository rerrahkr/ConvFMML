#ifndef _MODIFY_H_
#define _MODIFY_H_


#define INSERT_LIST(TYPE, POINTER)  insert_outlist(&head, rear, make_elist(TYPE, POINTER))


void modlastrhythm(void);
track_t *func_modify_track(track_t *head);
track_t *modify_track_data(track_t *track, track_t **head);
void func_modify_ccpc(track_t *track);
ccpc_t *modify_ccpc(note_t *nhead, ccpc_t *chead, int num);
ccpc_t *isccpcinrest(ccpc_t *predata, ccpc_t *nowdata, note_t *note, ccpc_t **head);
void delsomeccpcinrest(ccpc_t *p, ccpc_t **head);
ccpc_t *searchpre2ccpc(ccpc_t *p);
ccpc_t *ismodccpc(ccpc_t **predata, ccpc_t *nowdata, note_t *note, ccpc_t **head);
int ispart(int n);
char *convccpcvalue(ccpc_t *ccpc, int trackno);
void convert_volume(char *buf, char *cmd, unsigned char value, int range);
void setvolume_PMD(char *buf, unsigned char value, int trackno);
void setvolume_NRTDRV(char *buf, unsigned char value, int trackno);
void setpan_placeFMP7(char *buf, unsigned char value);
void setpan_place(char *buf, unsigned char value);
ccpc_t *delccpc(ccpc_t *del, ccpc_t **head);
void modify_note_and_rest_by_ccpc(track_t *track);
note_t *make_cutnote(note_t *prenote, pos_t inpos, int tieflgtype);
elist_t **create_eventlists(track_t *track);
elist_t *sort_events(track_t *track);
elist_t *make_elist(enum _LISTTYPE type, void *data);
pos_t getoutlistpos(elist_t *data);
elist_t *insert_outlist(elist_t **head_p, elist_t *cmp, elist_t *new);
void free_event_list(elist_t **seq);
void free_events(elist_t *seq);
unsigned int isinsert_tempo(elist_t **seq);
//int istrack(unsigned int num); -> convfmml.h
elist_t **modify_tempopart(elist_t **seq, unsigned int *t_partnum);
void cut_tempopart(elist_t *track);
void cut_and_sort_note(elist_t *note, elist_t *preinevent, pos_t in_pos, int tempocutflg);
elist_t **cut_note_by_measure(elist_t **seq, unsigned int t_partnum);
void search_cut_note(elist_t *cmp);


#endif /*_MODIFY_H_*/
