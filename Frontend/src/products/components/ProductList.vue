<template>
  <div>
    <div style="margin-bottom: 35px">
      <Toolbar class="card-border">
        <template #start>
          <Button
            label="Add new record"
            @click="onAdd"
            icon="pi pi-plus"
            class="p-ml-2"
          />
        </template>
        <template #end>
          <Button
            label="Select another Category"
            @click="onCategory"
            icon="pi pi-book"
            class="p-mr-2"
          />
        </template>
      </Toolbar>
    </div>

    <Card class="card-border">
      <template #loading>Loading customers data. Please wait...</template>
      <template #empty>No products found for selected category</template>
      <template #header>
        <Toolbar class="card-border">
          <template #start>
            <InputText
              v-model.trim="filter.productName"
              placeholder="Product name"
            />
            <Dropdown
              v-model="filter.supplier"
              :options="suppliers"
              optionLabel="companyName"
              placeholder="Select Supplier"
              class="p-ml-2 p-mr-2"
              :filter="true"
              :showClear="true"
            />
            <Button
              @click="onLoadData"
              icon="pi pi-search"
              class="p-ml-2"
              label="Filter"
            />
          </template>
        </Toolbar>
      </template>
      <template #content>
        <DataTable
          :scrollable="true"
          style="width: 100%"
          :value="products"
          sortMode="multiple"
          :paginator="true"
          :rows="10"
          :loading="loading"
          :rowsPerPageOptions="[10, 20, 50]"
          dataKey="productId"
          selectionMode="single"
          @row-select="onRowSelect"
          class="p-datatable-customers card-border"
        >
          <Column
            v-for="col of columns"
            :field="col.field"
            headerStyle="width: 200px"
            :header="col.header"
            :key="col.field"
            :sortable="true"
            :columnKey="col.field"
          >
          </Column>
        </DataTable>
      </template>
    </Card>
  </div>
</template>

<script>
import Toolbar from "primevue/toolbar";
import Button from "primevue/button";
import Card from "primevue/card";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";

import CategoryService from "../services/category.service";
import SupplierService from "../services/supplier.service";

export default {
  components: {
    Toolbar,
    Button,
    Card,
    DataTable,
    Column,
    InputText,
    Dropdown,
  },
  data() {
    return {
      categoryService: new CategoryService(),
      supplierService: new SupplierService(),
      products: [],
      suppliers: [],
      loading: true,
      categoryId: this.$route.params.categoryId,
      filter: {
        productName: "",
        supplier: "",
      },
      columns: [
        //productId
        { field: "productName", header: "Product Name" },
        { field: "quantityPerUnit", header: "Quantity per unit" },
        { field: "reorderLevel", header: "Reorder level" },
        { field: "unitPrice", header: "Unit Price" },
        { field: "unitsInStock", header: "Units in Stock" },
        { field: "unitsOnOrder", header: "Units on Order" },
        // supplierId
        { field: "supplierName", header: "Supplier name" },
        // categoryId
        { field: "categoryName", header: "Category name" },
        { field: "discontinued", header: "Discounted" },
        { field: "orderDetailCount", header: "Order Details Count" }
      ],
    };
  },
  created() {
    this.loading = true;
    this.supplierService
      .getSuppliers()
      .then((response) => (this.suppliers = response))
      .catch((error) => console.error(error))
      .finally(() => {});
    this.onLoadData();
  },
  methods: {
    onLoadData() {
      this.loading = true;

      let filter = {
        productName: this.filter?.productName,
        supplierId: this.filter?.supplier?.supplierId
      };

      this.categoryService
        .getProductsByCategoryId(this.categoryId, filter)
        .then((response) => (this.products = response))
        .catch((error) => console.error(error))
        .finally(() => {
          console.log(this.filter);
          console.log(this.products);
          this.loading = false;
        });
    },
    onAdd() {
      this.$router.push(`/categories/${this.categoryId}/products/new`);
    },
    onCategory() {
      this.$router.push("/categories");
    },
    onRowSelect(event) {
      this.$router.push(
        `/categories/${this.categoryId}/products/${event.data.productId}`
      );
    }
  },
};
</script>
