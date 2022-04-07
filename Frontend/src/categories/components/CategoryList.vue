<template>
  <div>
    <BlockUI :blocked="loading" :fullScreen="true">
      <ProgressBar v-if="loading" mode="indeterminate" style="height: 0.5em" />
      <Toolbar style="margin-bottom: 20px">
        <template #start>
          <h1>Select a Category</h1>
        </template>
        <template #end>
          <Button
            label="Add new category"
            icon="pi pi-plus"
            class="p-button-success"
            @click="onAdd"
          />
        </template>
      </Toolbar>
      <div class="p-grid">
        <div
          v-for="category in categories"
          :key="category.categoryId"
          class="p-md-4 p-sm-6 p-lg-4 p-xs-12"
        >
          <Card class="p-m-2 category-card">
            <template #header>
              <img alt="user header" v-bind:src="category.picture" />
            </template>
            <template #title>
              {{ category.categoryName }} ({{ category.productCount }})
            </template>
            <template #content>
              <div class="text-description">
                {{ category.description }}
              </div>
            </template>
            <template #footer>
              <Button
                label="Products"
                icon="pi pi-table"
                class="p-button-secondary"
                @click="onProductsByCategoryId(category.categoryId)"
              />
              <Button
                label="Edit"
                icon="pi pi-pencil"
                class="p-button-primary p-ml-2"
                @click="onEdit(category.categoryId)"
              />
            </template>
          </Card>
        </div>
      </div>
    </BlockUI>
  </div>
</template>

<script>
/* #region Components and Services */
import Toolbar from "primevue/toolbar";
import Card from "primevue/card";
import BlockUI from "primevue/blockui";
import ProgressBar from "primevue/progressbar";
import Button from "primevue/button";

import CategoryService from "../services/category.service";
/* #endregion */

export default {
  components: {
    Toolbar,
    Card,
    BlockUI,
    ProgressBar,
    Button,
  },
  data() {
    return {
      categoryService: new CategoryService(),
      categories: [],
      loading: true,
    };
  },
  created() {
    this.onLoadCategories();
  },
  methods: {
    onLoadCategories() {
      this.categoryService
        .getCategories()
        .then((response) => {
          this.categories = response;
        })
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    onAdd() {
      this.$router.push(`/categories/new`);
    },
    onEdit(categoryId) {
      this.$router.push(`/categories/${categoryId}`);
    },
    onProductsByCategoryId(categoryId) {
      this.$router.push(`/categories/${categoryId}/products`);
    },
  },
};
</script>

<style scoped>
.text-description {
   width: 250px;
   white-space: nowrap;
   overflow: hidden;
   text-overflow: ellipsis;
 }
</style>
