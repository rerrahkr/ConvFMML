#ifndef _ANALYZE_H_
#define _ANALYZE_H_


track_t *func_analyze(struct _CHUNK *chunkdata, unsigned short num);
track_t *analyze_chunkdata(const struct _CHUNK chunkdata, track_t *track, track_t **head, int num);
track_t *addtrackcell(track_t *pre, track_t **head, int num);
//track_t *deltrackcell(track_t *del, track_t **head); -> convfmml.h
void free_track_list(track_t *list);
int calcVLQshiftbyte(byte_t *byte, unsigned int *VLQ);
//int cmppos(pos_t a, pos_t b); -> convfmml.h
pos_t calcpos(pos_t pos, unsigned int delta, rhythm_t **rhythm);
unsigned int calcmmltick(unsigned int miditick);
marker_t *make_marker_list(marker_t *pre, pos_t pos, byte_t *data, unsigned int size);
void free_marker_list(marker_t *list);
tempo_t *make_tempo_list(tempo_t *prelist, pos_t pos);
void free_tempo_list(tempo_t *list);
tempo_t *settempodata(tempo_t *tempo, pos_t pos, byte_t *data);
rhythm_t *make_rhythm_list(rhythm_t *prelist, pos_t pos);
void free_rhythm_list(rhythm_t *list);
rhythm_t *setrhythmdata(rhythm_t *rhythm, pos_t oldpos, pos_t newpos, byte_t num, byte_t denom_pow2, byte_t beatclock, byte_t hdm_semi);
keysig_t *make_keysig_list(keysig_t *prelist, pos_t pos, byte_t sig_num, byte_t minorflg);
void free_keysig_list(keysig_t *list);
//void free_metalists(void); -->convfmml.h
//note_t *make_note_list(note_t *prelist, pos_t pos); -->convfmml.h
//note_t *delnote(note_t *del, note_t **head); -->convfmml.h
//void free_note_list(note_t *list); -->convfmml.h
track_t *setnoteon(track_t *track, pos_t pos, byte_t ch, byte_t key, byte_t velocity, track_t **head, int num);
track_t *setnoteoff(track_t *track, pos_t pos, byte_t ch, byte_t key);
ccpc_t *make_ccpc_list(ccpc_t *prelist, pos_t pos, ccpc_t **head);
//void free_ccpc_list(ccpc_t *list); -->convfmml.h
ccpc_t *copyccpclist(ccpc_t *src);
ccpc_t *setccpcdata(ccpc_t *ccpc, pos_t pos, enum _CCPCTYPE type, byte_t ch, byte_t value, ccpc_t **head);
void ismodify_notepos(note_t *cmp, pos_t oldpos, pos_t newpos);
void ismodify_ccpcpos(ccpc_t *cmp, pos_t oldpos, pos_t newpos);


#endif /*_ANALYZE_H_*/
