#ifndef _INPUT_H_
#define _INPUT_H_


char *getfileextend(char *filename);
struct _CHUNK *load_midi(char *infilename, unsigned short *num);
int islendian(void);
void convert_lendian(void *s, size_t size);
void free_chunkdata(struct _CHUNK *data, unsigned short num);


#endif /*_INPUT_H_*/
