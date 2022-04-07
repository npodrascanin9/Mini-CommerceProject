<template>
  <div>
    <BlockUI :blocked="loading" :fullScreen="true">
    <ProgressBar v-if="loading" mode="indeterminate" style="height: 0.5em" />
    <Toolbar>
      <template #start>
        <Button label="Submit" icon="pi pi-save" @click="onSubmit" />
      </template>
      <template #end>
        <Button
          v-show="!isNew"
          label="Delete"
          icon="pi pi-trash"
          class="p-button-danger"
          @click="onDelete"
        />
        <Button label="Cancel" icon="pi pi-times" class="p-button-secondary p-ml-2" @click="onCategories" />
      </template>
    </Toolbar>

    <div class="p-mt-4">
      <Card>
        <template #content>
          <div class="p-fluid p-formgrid p-grid">
            <!-- #region form controls -->
            <div class="p-field p-col-12">
              <label v-bind:for="form.categoryName.name">{{
                form.categoryName.title
              }}</label>
              <InputText
                v-bind:id="form.categoryName.name"
                v-bind:type="form.categoryName.type"
                v-model.trim="form.categoryName.value"
              />
              <p
                v-if="!form.categoryName.validation.required.isValid"
                class="error"
              >
                {{ form.categoryName.validation.required.message }}
              </p>
              <p v-if="!form.categoryName.validation.maxLength.isValid" class="error">
                  {{ form.categoryName.validation.maxLength.message }}
              </p>
            </div>
            <div class="p-field p-col-12">
              <label v-bind:for="form.description.name">{{
                form.description.title
              }}</label>
              <Textarea v-model.trim="form.description.value" v-bind:name="form.description.name" :autoResize="true" rows="5" cols="30" />
              <p
                v-if="!form.description.validation.required.isValid"
                class="error"
              >
                {{ form.description.validation.required.message }}
              </p>
            </div>
            <div class="p-field p-col-12">
              <FileUpload
                name="demo[]"
                mode="basic"
                :customUpload="true"
                :fileLimit="1"
                @uploader="onUpload"
              />

              <img v-bind:src="picture.imgBase64" width="200px" height="200px" class="p-mt-3">

            </div>
            <!-- #endregion -->
          </div>
        </template>
      </Card>
    </div>
    </BlockUI>
  </div>
</template>

<script>
/* #region Components and Services */
import Toolbar from "primevue/toolbar";
import Button from "primevue/button";
import Card from "primevue/card";
import InputText from "primevue/inputtext";
import FileUpload from "primevue/fileupload";
import BlockUI from "primevue/blockui";
import ProgressBar from "primevue/progressbar";
import Textarea from 'primevue/textarea';

import CategoryService from "../services/category.service.js";
/* #endregion */

export default {
  name: "CategoryDetail",
  components: {
    Toolbar,
    Button,
    Card,
    InputText,
    FileUpload,
    BlockUI,
    ProgressBar,
    Textarea
  },
  data() {
    return {
      id: this.$route.params.id,
      isNew: false,
      loading: true,
      service: new CategoryService(),
      picture: {
        bytes: null,
        imgBase64: null,
      },
      input: {
        categoryId: null,
        categoryName: null,
        description: null,
        picture: null,
      },
      form: {
        isValid: true,
        categoryName: {
          value: "",
          name: "categoryName",
          type: "text",
          title: "Category Name",
          validation: {
            required: {
              isValid: true,
              message: "Category Name is required!",
            },
            maxLength: {
                isValid: true,
                message: "Category name cannot have more than 15 characters!"
            }
          },
        },
        description: {
          value: "",
          name: "description",
          type: "text",
          title: "Description",
          validation: {
            required: {
              isValid: true,
              message: "Description is required",
            },
          },
        },
      },
    };
  },
  created() {
    this.isNew = this.id === "new";
    if (!this.isNew) {
      this.input.categoryId = this.id = Number(this.id);
      this.initData();
    } else {
        this.loading = false;
    }
  },
  mounted() {},
  methods: {
    initData() {
      this.loading = true;
      this.service
        .getCategoryById(this.id)
        .then((response) => this.initForm(response))
        .catch((error) => console.error(error))
        .finally(() => {
          this.loading = false;
        });
    },
    initForm(data) {
      this.picture = {
        bytes: data.picture.bytes,
        imgBase64: data.picture.imgBase64,
      };

      (this.form.categoryName.value = data.categoryName),
        (this.form.description.value = data.description),
        (this.input.picture = {
          bytes: data.picture.bytes,
          imgBase64: data.picture.imgBase64,
        });
    },
    onUpload(event) {
      let formData = new FormData();
      formData.append("file", event.files[0]);

      this.loading = true;
      this.service
        .uploadImage(formData)
        .then((response) => {
          this.picture = {
            bytes: response.bytes,
            imgBase64: response.imgBase64,
          };
        })
        .catch((error) => console.error(error))
        .finally(() => {
          this.loading = false;
        });
    },
    onCategories() {
      this.$router.push("/categories");
    },
    onDelete() {
      this.loading = true;
      this.service
        .deleteCategoryById(Number(this.id))
        .then(() => {
            this.loading = false;
        })
        .catch((error) => console.error(error))
        .finally(() => {
          this.$router.push("/categories");
        });
    },
    onSubmit() {
      this.validate();
      if (!this.form.isValid) {
        return;
      }

      this.initInput();
      console.log(this.input);

      this.loading = true;
      if (this.isNew) {
        this.service
          .createCategory(this.input)
          .then((response) => {
            console.log(response);
            this.initForm(response);
            this.loading = false;
          })
          .catch((error) => {
              this.loading = false;
              console.error(error);
          })
          .finally(() => {
            this.$router.push(`/categories`);
          });
      } else {
        console.log(this.input);
        this.service
          .updateCategory(Number(this.id), this.input)
          .then(() => {
            this.loading = false;
          })
          .catch((error) => console.error(error))
          .finally(() => {
            this.$router.push(`/categories`);
          });
      }
    },
    validate() {
      //   const validation = {
      //     categoryName: this.form.categoryName.validation.required.isValid = this.form.categoryName.value ? true : false,
      //     description: this.form.description.validation.required.isValid = this.form.description.value ? true : false
      //   }
      let validations = [
        (this.form.categoryName.validation.required.isValid = this.form
          .categoryName.value
          ? true
          : false)
        ,(this.form.categoryName.validation.maxLength.isValid = this.form.categoryName.value?.length <= 15)
        ,(this.form.description.validation.required.isValid = this.form
          .description.value
          ? true
          : false),
      ];

      this.form.isValid = validations.every((v) => v === true);
    },
    initInput() {
      this.input = {
        categoryId: this.isNew ? null : Number(this.id),
        categoryName: this.form.categoryName.value,
        description: this.form.description.value,
        picture: this.picture?.bytes,
      };
    },
  },
};
</script>

<style scoped>
.error {
  color: #f00;
  font-weight: bold;
}
</style>
