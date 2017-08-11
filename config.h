#ifndef _CONFIG_H_
#define _CONFIG_H_


#define BUF_SIZE 100

#define STRCMP(STR)         strncmp(buf, STR, strlen(STR))
#define ISCONF(CONF, A, B)  ( ((CONF) >= (A) && (CONF) <= (B))? 1 : 0 )
#define EPRINTI(SEC, KEY)   fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の[" SEC "]部、\"" KEY "\"の値が間違っています。\n")
#define EPRINTS(SEC, KEY)   fprintf(stderr, "[ERROR] \"ConvFMML.ini\"の[" SEC "]部、\"" KEY "\"が設定されていません。\n")


enum _PANPLACE {
  LEFT,
  RIGHT
};


void load_config(char *path);
void init_config(void);
void set_config(char *buf);
char *sub_confstr(char *buf);
void set_panpoint(char *buf, enum _PANPLACE place);
void set_number_track(char *buf);
void sort_number_track(int n);
void set_partname(char *buf);
void set_auto_partname(void);
int check_config(void);
void set_modecmds(void);
void set_track_and_partsize(void);
//void free_config(void); -> convfmml.h


#endif /*_CONFIG_H_*/
